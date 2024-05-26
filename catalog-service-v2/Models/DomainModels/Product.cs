namespace catalog_service_v2.Models.DomainModels;

using System;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int Sold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Disabled { get; set; }
    public string ImageId { get; set; }
    public List<Image> Images { get; set; }
    public List<Category> Categories { get; set; }

}

public class ElasticSearchProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int Sold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Disabled { get; set; }
    public string ImageId { get; set; }
    public List<ElasticSearchImage> Images { get; set; }
    public List<ElasticSearchCategory> Categories { get; set; }
}