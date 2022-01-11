using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Npgsql;

namespace prikapp
{
    public class LocationData
    {
        public string Place {get; set;}
        public string LocationName {get; set;}
        public string Address {get; set;}
        public string Postalcode {get; set;}
        public string OpeningHours {get; set;}
        public string Particularities {get; set;}
    }

    public class Card
    {
        public int Id {get; set;}
        public short Type {get; set;}
        public string Title {get; set;}
        public string Description {get; set;}
        public string Content {get; set;}

        public Card(int id, short type, string title, string description, string content)
        {
            Id = id;
            Type = type;
            Title = title;
            Description = description;
            Content = content;
        }
    }
    public class UserInformation
    {
        public string Username {get; set;}
        public string Password {get; set;}
    }

    public class Startup
    {
        public List<string> Tokens {get; set;} = new List<string>();
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles()
               .UseStaticFiles()
               .UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/html");
                    await context.Response.WriteAsync(System.IO.File.ReadAllText(@"./wwwroot/index.html"));
                });

                endpoints.MapGet("/locations.json", async context =>
                {
                    var acceptedEncoding = context.Request.Headers["Accept-Encoding"];
                    System.Console.WriteLine("Request made to /locations.json with accepted encoding: " + acceptedEncoding);
                    
                    string starSiteUrl = @"https://www.star-shl.nl/bloedafname-locatie/";
                    string pageHTML = DownloadPage(starSiteUrl);
                    List<LocationData> result = StripLocations(pageHTML);
                    string jsonString = JsonSerializer.Serialize(result);

                    context.Response.Headers.Add("Content-Type", "application/json");
                    
                    await context.Response.WriteAsync(jsonString);
                });

                endpoints.MapGet("/postcodes.json", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "application/json");
                    await context.Response.WriteAsync(File.ReadAllText("./postcodes.json"));
                });

                endpoints.MapGet("/{**any}", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/html");
                    await context.Response.WriteAsync(System.IO.File.ReadAllText(@"./wwwroot/index.html"));
                });

                endpoints.MapGet("/cards.json", async context =>
                {
                    var cards = new List<Card>();

                    await RunQuery("SELECT * FROM cards", async cmd => {
                        await using (var reader = await cmd.ExecuteReaderAsync())
                            while (await reader.ReadAsync())
                                cards.Add(new Card(reader.GetInt32(0), reader.GetInt16(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
                    });

                    context.Response.Headers.Add("Content-Type", "application/json");
                    await context.Response.WriteAsync(JsonSerializer.Serialize(cards));
                });
                
                endpoints.MapPost("/saveCard", async context =>
                {
                    // extract the card data from the request body
                    var card = await context.Request.ReadFromJsonAsync<Card>();
                    await RunQuery("UPDATE Cards SET type = @type, title = @title, description = @description, content = @content WHERE id = @id", async cmd =>
                    {
                        cmd.Parameters.AddWithValue("type", card.Type);
                        cmd.Parameters.AddWithValue("title", card.Title);
                        cmd.Parameters.AddWithValue("description", card.Description);
                        cmd.Parameters.AddWithValue("content", card.Content);
                        cmd.Parameters.AddWithValue("id", card.Id);
                        await cmd.ExecuteNonQueryAsync();
                    });
                });
                endpoints.MapPost("/login", async context =>
                {
                    // extract the card data from the request body
                    var user = await context.Request.ReadFromJsonAsync<UserInformation>();
                    await RunQuery("SELECT username FROM Users WHERE username = @username AND password = @password", async cmd =>
                    {
                        cmd.Parameters.AddWithValue("username", user.Username);
                        cmd.Parameters.AddWithValue("password", user.Password);
                        var userCount = await cmd.ExecuteNonQueryAsync();
                        Console.WriteLine(userCount);
                        Console.WriteLine(user.Username);
                        Console.WriteLine(user.Password);
                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string guid = Guid.NewGuid().ToString();
                                Console.WriteLine(guid);
                                Tokens.Add(guid);
                                await context.Response.Body.WriteAsync(System.Text.Encoding.ASCII.GetBytes(guid));
                            }
                        }
                    });
                });
            });
        }
        
        public static string DownloadPage(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            return result;
        }

        public static List<LocationData> StripLocations(string htmlPage)
        {
            List<LocationData> locations = new List<LocationData>();
            var matches = Regex.Matches(htmlPage, @"<tbody(?: class=""table-hover"")?>(.*)<\/tbody>", RegexOptions.Singleline);

            foreach (var match in matches)
            {
                var dataMatches = Regex.Matches(match.ToString(), @"<tr class="""">.*?<th scope=""row"">(.*?)<\/th>.*?<td>(.*?)<\/td>.*?<td>(.*?)<\/td>.*?<td>(.*?)<\/td>.*?<td>(.*?)<\/td>.*?(?<=<!--).*?<td>(.*?)<\/td>.*?<\/tr>", RegexOptions.Singleline);
                foreach (Match dataMatch in dataMatches)
                {
                    LocationData location = new LocationData();
                    GroupCollection groups = dataMatch.Groups;

                    // set fields in location object
                    location.Place = groups[1].ToString();
                    location.LocationName = groups[2].ToString();
                    location.Address = groups[3].ToString();
                    location.Postalcode = groups[4].ToString();
                    location.OpeningHours = Regex.Match(groups[5].ToString().Substring(26, groups[5].ToString().Length-26), @"(.*?)(?=</p>)").ToString();
                    location.Particularities = groups[6].ToString();
                    
                    locations.Add(location);
                }
            }

            return locations;
        }

        public static async Task RunQuery(string sql, Func<NpgsqlCommand, Task> action)
        {
            var url = new Uri(Environment.GetEnvironmentVariable("DATABASE_URL"));
            var userInfo = url.UserInfo.Split(':');
            await using var conn = new NpgsqlConnection($"Host={url.Host};Username={userInfo[0]};Password={userInfo[1]};Database={url.LocalPath.Substring(1)};SslMode=Require;TrustServerCertificate=true;");
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand(sql, conn))
                await action(cmd);
        }
    }
}