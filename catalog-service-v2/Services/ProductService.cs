using catalog_service_v2.Models.DomainModels;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace catalog_service_v2.Services;

public interface IProductService
{
    public Task<IEnumerable<ElasticSearchProduct>> GetProductsAsync(int? page, int? pageSize, string? search, string? sort, int? minPrice, int? maxPrice);
    public Task AddProduct(Product product);


    // Test if elastic search is up and running
    public Task<bool> PingAsync();
    // Check product count
    public Task<int> GetProductCountAsync();

    public Task<Product> GetProductAsync(int productId);
}

public class ProductService : IProductService
{
    private readonly ElasticsearchClient _client;

    private readonly string _productIndex;

    /// <summary>
    /// Get all products from Elasticsearch
    ///  - From() : Skip the first n results
    ///  - Size() : Limit the number of results
    /// </summary>
    public async Task<IEnumerable<ElasticSearchProduct>> GetProductsAsync(int? page, int? pageSize, string? search, string? sort, int? minPrice, int? maxPrice)
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
            .Index(_productIndex)
            .Size(10000)
            .Query(q => q
                .Wildcard(w => w
                    .Field(f => f.Name)
                    .Value($"*{search}*")
                    .CaseInsensitive(true)
                ))
        );

        if (response.IsValidResponse)
        {
            IEnumerable<ElasticSearchProduct> products = response.Documents;

            if (minPrice != null) products = products.Where(p => p.Price >= minPrice);
            if (maxPrice != null) products = products.Where(p => p.Price <= maxPrice);

            switch (sort)
            {
                case "name_asc":
                    products = products.OrderBy(a => a.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(a => a.Name);
                    break;
                case "price_asc":
                    products = products.OrderBy(a => a.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(a => a.Price);
                    break;
                default:
                    break;
            }

            products = products.Skip(from).Take(size);

            return products;
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
            .Index(_productIndex)
            .Query(q => q.MatchAll()));
        return (int)count.Count;
    }

    public async Task<Product> GetProductAsync(int productId)
    {
        var searchResponse = await _client.SearchAsync<Product>(s => s
            .Index(_productIndex)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Id)
                    .Query(productId.ToString())
                )
            )
        );

        if (searchResponse.Documents.Any())
        {
            return searchResponse.Documents.First();
        }
        return null; // Document not found or other issue
    }


    public ProductService(ElasticsearchClient client)
    {
        _client = client;
        _productIndex = Environment.GetEnvironmentVariable("PRODUCT_INDEX") ?? "products";
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