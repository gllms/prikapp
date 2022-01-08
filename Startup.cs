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

    public class Startup
    {
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
            });
        }
    }
}