using catalog_service_v2.Models.DomainModels;

namespace catalog_service_v2.Models.ApiModels.ProductApiModels;

public class GetAllProductsResponse
{
    public int TotalProducts { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string Search { get; set; } = null!;
    public string Sort { get; set; } = null!;
    public List<Product> Products { get; set; } = null!;
}