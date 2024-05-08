using catalog_service_v2.Models.DomainModels;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace catalog_service_2.Configuration;

public class ElasticSearchConfig
{
    public static async void Configure(IServiceCollection services, string userName, string password)
    {
        // ElasticSearch
        /* If run through docker-compose, use the following nodes
        var nodes = new Uri[]
        {
            new Uri("http://elasticsearch1:9200"),
            new Uri("http://elasticsearch2:9200"),
            new Uri("http://elasticsearch3:9200")
        };
        */
        
        var nodes = new Uri[]
        {
            new Uri("http://localhost:9200"),
        };
        
        var pool = new StaticNodePool(nodes);


        var settings = new ElasticsearchClientSettings(pool)
            // Default Mapping
            .DefaultMappingFor<Product>(m => m
                .IndexName("product-logs")
            )
            // Setup
            .Authentication(new BasicAuthentication("elastic", "changeme"))
            .EnableDebugMode() // Optional: Enables detailed logging for debugging
            .PrettyJson() // Optional: Formats JSON output to be more readable
            .RequestTimeout(TimeSpan.FromMinutes(2));
            

        var client = new ElasticsearchClient(settings);
        
        var settings2 = client.Indices.CreateAsync("product-logs-2");

        // Services
        services.AddSingleton(client);
    }
}