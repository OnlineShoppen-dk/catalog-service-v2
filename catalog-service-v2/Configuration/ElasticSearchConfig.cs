﻿using catalog_service_v2.Models.DomainModels;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace catalog_service_v2.Configuration;

public class ElasticSearchConfig
{
    public static async void Configure(IServiceCollection services, string userName, string password)
    {
        var productQueue = Environment.GetEnvironmentVariable("PRODUCT_INDEX") ?? "products";
        
        var nodes = new Uri[]
        {
            new Uri("http://elasticsearch:9200"),
        };
        
        var pool = new StaticNodePool(nodes);


        var settings = new ElasticsearchClientSettings(pool)
            // Default Mapping
            .DefaultMappingFor<Product>(m => m
                .IndexName(productQueue)
            )
            // Setup
            .Authentication(new BasicAuthentication(userName, password))
            .EnableDebugMode() // Optional: Enables detailed logging for debugging
            .PrettyJson() // Optional: Formats JSON output to be more readable
            .RequestTimeout(TimeSpan.FromMinutes(2));
            

        var client = new ElasticsearchClient(settings);
        
        var settings2 = client.Indices.CreateAsync(productQueue);

        // Services
        services.AddSingleton(client);
    }
}