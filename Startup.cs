using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
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
        public int Password {get; set;}
    }

    public class Startup
    {
        public List<string> Tokens {get; set;} = new List<string>();
        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddResponseCompression(options => 
            {
                options.EnableForHttps = true;
            });

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression()
               .UseDefaultFiles()
               .UseStaticFiles()
               .UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/html");
                    await context.Response.WriteAsync(System.IO.File.ReadAllText(@"./wwwroot/index.html"));
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
                    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) && Tokens.Contains(authHeader.ToString()))
                    {
                        try
                        {
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
                            await context.Response.WriteAsync("Success");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            await context.Response.WriteAsync("Error");
                        }
                    }
                    else
                    {
                        await context.Response.WriteAsync("Unauthorized");
                    }
                });

                endpoints.MapPost("/login", async context =>
                {
                    try
                    {
                        var user = await context.Request.ReadFromJsonAsync<UserInformation>();
                        await RunQuery("SELECT username FROM Users WHERE username = @username AND password = @password", async cmd =>
                        {
                            cmd.Parameters.AddWithValue("username", user.Username);
                            cmd.Parameters.AddWithValue("password", user.Password);
                            var userCount = await cmd.ExecuteNonQueryAsync();
                            await using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    string guid = Guid.NewGuid().ToString();
                                    Tokens.Add(guid);
                                    await context.Response.Body.WriteAsync(System.Text.Encoding.ASCII.GetBytes(guid));
                                }
                            }
                        });
                    }
                    catch {}
                });

                endpoints.MapPost("/checkToken", async context =>
                {
                    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) && Tokens.Contains(authHeader.ToString()))
                    {
                        await context.Response.WriteAsync("Success");
                    }
                    else
                    {
                        await context.Response.WriteAsync("Unauthorized");
                    }
                });

                endpoints.MapPost("/logOut", async context =>
                {
                    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) && Tokens.Contains(authHeader.ToString()))
                    {
                        Tokens.Remove(authHeader.ToString());
                        await context.Response.WriteAsync("Success");
                    }
                    else
                    {
                        await context.Response.WriteAsync("Error");
                    }
                });

                endpoints.MapPost("/createCard", async context =>
                {
                    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) && Tokens.Contains(authHeader.ToString()))
                    {
                        try
                        {
                            await RunQuery(@"INSERT INTO Cards (type, title, description, content) VALUES (0, '', '', '{}') RETURNING id", async cmd =>
                            {
                                await using (var reader = await cmd.ExecuteReaderAsync())
                                    if (await reader.ReadAsync())
                                        await context.Response.WriteAsync(reader.GetInt32(0).ToString());
                            });
                        }
                        catch {}
                    }
                });

                endpoints.MapPost("/deleteCard", async context =>
                {
                    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) && Tokens.Contains(authHeader.ToString()) && context.Request.Headers.TryGetValue("Id", out var id))
                    {
                        try
                        {
                            await RunQuery("DELETE FROM Cards WHERE id = @id", async cmd =>
                            {
                                cmd.Parameters.AddWithValue("id", Int32.Parse(id));
                                await cmd.ExecuteNonQueryAsync();
                                await context.Response.WriteAsync("Success");
                            });
                        }
                        catch {}
                    }
                });
            });
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