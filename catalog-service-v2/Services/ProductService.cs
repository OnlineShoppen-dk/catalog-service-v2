using catalog_service_v2.Models.DomainModels;
using Elastic.Clients.Elasticsearch;

namespace catalog_service_v2.Services;

public interface IProductService
{
    public Task<IEnumerable<ElasticSearchProduct>> GetProductsAsync(int? page, int? pageSize, string? search, string? sort);
    public Task AddProduct(Product product);


    // Test if elastic search is up and running
    public Task<bool> PingAsync();
    // Check product count
    public Task<int> GetProductCountAsync();
}

public class ProductService : IProductService
{
    private readonly ElasticsearchClient _client;

    private const string IndexName = "product-logs";

    /// <summary>
    /// Get all products from Elasticsearch
    ///  - From() : Skip the first n results
    ///  - Size() : Limit the number of results
    /// </summary>
    public async Task<IEnumerable<ElasticSearchProduct>> GetProductsAsync(int? page, int? pageSize, string? search, string? sort)
    {
        // Setup Pagination, optimize later
        const int defaultPage = 1;
        const int defaultPageSize = 10;

        page ??= defaultPage;
        pageSize ??= defaultPageSize;

        var from = 0;
        if (page > 1)
        {
            from = (int)((page * pageSize) - pageSize);
        }

        var size = pageSize ?? defaultPageSize;
        
        var response = await _client.SearchAsync<ElasticSearchProduct>(s => s
            .Index(IndexName)
            .From(from)
            .Size(size)
            .Query(q => q
                .Wildcard(w => w
                    .Field(f => f.name)
                    .Value($"*{search}*")
                    .CaseInsensitive(true)
                ))
        );

        if (response.IsValidResponse)
        {
            return response.Documents;
        }
        throw new Exception("Search failed or no results found");
    }

    /// <summary>
    /// For debugging purposes
    /// </summary>
    /// <param name="product"></param>
    public async Task AddProduct(Product product)
    {
        var response = await _client.IndexAsync(product);

        if (!response.IsValidResponse)
        {
            Console.WriteLine("Failed to add product");
            Console.WriteLine(response.DebugInformation);
        }
    }

    /// <summary>
    /// Test if Elasticsearch is up and running
    /// </summary>
    /// <returns></returns>
    public async Task<bool> PingAsync()
    {
        var response = await _client.PingAsync();
        return response.IsValidResponse;
    }

    /// <summary>
    /// Get the total count of products in Elasticsearch
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetProductCountAsync()
    {
        var count = await _client.CountAsync(c => c
            .Index(IndexName)
            .Query(q => q.MatchAll()));
        return (int)count.Count;
    }


    public ProductService(ElasticsearchClient client)
    {
        _client = client;
    }
}

/* OLD
 public async Task<List<Product>> GetProductsAsync(int? page, int? pageSize, string? search, string? sort)
    {
        // Setup Pagination, optimize later
        const int defaultPage = 1;
        const int defaultPageSize = 10;

        page ??= defaultPage;
        pageSize ??= defaultPageSize;

        var from = 0;
        if (page > 1)
        {
            from = (int)((page * pageSize) - pageSize);
        }

        var size = pageSize ?? defaultPageSize;

        var response = await _client.SearchAsync<Product>(s => s
            .Index("product-logs")
            .From(from)
            .Size(size)
            .Query(q => q
                .Wildcard(w => w
                    .Field(f => f.Name)
                    .Value($"*{search}*")
                    .CaseInsensitive(true)
                )
            )
            // Source : https://discuss.elastic.co/t/using-sort-api-via-elastic-clients-elasticsearch-8-1-0-net/330908
            .Sort(so =>
            {
                switch (sort)
                {
                    case "name_asc":
                        so.Field(f => f.Name.Suffix("keyword"), new FieldSort { Order = SortOrder.Asc });
                        break;
                    case "name_desc":
                        so.Field(f => f.Name.Suffix("keyword"), new FieldSort { Order = SortOrder.Desc });
                        break;
                    case "price_asc":
                        so.Field(f => f.Price, new FieldSort { Order = SortOrder.Asc });
                        break;
                    case "price_desc":
                        so.Field(f => f.Price, new FieldSort { Order = SortOrder.Desc });
                        break;
                    default:
                        so.Field(f => f.Name.Suffix("keyword"), new FieldSort { Order = SortOrder.Asc });
                        break;
                }
            }));

        if (response.IsValidResponse)
        {
            Console.WriteLine("rip");
            var result = response.Documents.ToList();
            return result;
        }

        Console.WriteLine("Not valid response");

        throw new Exception("Search failed or no results found");
    }
*/