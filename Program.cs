using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.IO.Compression;

namespace prikapp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // request files from star and create compressed versions
            string starSiteUrl = @"https://www.star-shl.nl/bloedafname-locatie/";
            string pageHTML = DownloadPage(starSiteUrl);
            List<LocationData> result = StripLocations(pageHTML);
            string jsonString = JsonSerializer.Serialize(result);

            File.WriteAllText("./wwwroot/build/locations.json", jsonString);

            // start server
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

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
