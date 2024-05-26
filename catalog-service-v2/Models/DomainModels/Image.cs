namespace catalog_service_v2.Models.DomainModels;

public class Image
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string Alt { get; set; } = null!;
}

public class ElasticSearchImage
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string Alt { get; set; } = null!;
}