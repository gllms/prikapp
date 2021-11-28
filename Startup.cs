using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/html");
                    await context.Response.WriteAsync(System.IO.File.ReadAllText(@"./wwwroot/index.html"));
                });
                endpoints.MapGet("/locations.json", async context =>
                {
                    // TODO: request locations from star and convert to json
                    string starSiteUrl = @"https://www.star-shl.nl/bloedafname-locatie/";
                    string pageHTML = DownloadPage(starSiteUrl);
                    List<LocationData> result = StripLocations(pageHTML);
                    string jsonString = JsonSerializer.Serialize(result);

                    context.Response.Headers.Add("Content-Type", "application/json");
                    await context.Response.WriteAsync(jsonString);
                });
                endpoints.MapGet("/{**unknown}", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/html");
                    await context.Response.WriteAsync(System.IO.File.ReadAllText(@"./wwwroot/index.html"));
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
    }
}