namespace catalog_service_v2.Models.DomainModels;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class ElasticSearchCategory
{
    public int id { get; set; }
    public string name { get; set; } = null!;
    public string description { get; set; } = null!;
}