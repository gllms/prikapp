using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
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
                    var acceptedEncoding = context.Request.Headers["Accept-Encoding"];
                    System.Console.WriteLine("Request made to /locations.json with accepted encoding: " + acceptedEncoding);

                    if (acceptedEncoding.Contains("gzip")) 
                    {
                        context.Response.Headers.Add("Content-Encoding", "gzip");
                        await context.Response.SendFileAsync("Locations.gz");
                    } 
                    else 
                    {
                        context.Response.Headers.Add("Content-Type", "application/json");
                        await context.Response.WriteAsync(File.ReadAllText("Locations.json"));
                    }
                    
                });
                endpoints.MapGet("/postcodes.json", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "application/json");
                    await context.Response.WriteAsync(File.ReadAllText("./postcodes.json"));
                });
                endpoints.MapGet("/{**unknown}", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/html");
                    await context.Response.WriteAsync(System.IO.File.ReadAllText(@"./wwwroot/index.html"));
                });
            });
        }
    }
}