﻿using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync(database: "SearchDb",
                settings: MongoClientSettings.
                    FromConnectionString(
                        app.Configuration
                        .GetConnectionString("MongoDbConnection")));
            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();
            if (count == 0)
            {
                Console.WriteLine("No data - will attempt to seed");
                var itemData = await File.ReadAllTextAsync(path: "Data/auctions.json");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var items = JsonSerializer.Deserialize<List<Item>>(json: itemData, options: options);
                await DB.SaveAsync(entities: items);
            }
        }
    }
}
