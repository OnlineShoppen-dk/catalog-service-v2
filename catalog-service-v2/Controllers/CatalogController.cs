﻿using catalog_service_2.Services;
using catalog_service_v2.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;

namespace catalog_service_2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    
    private readonly IProductService _productService;

    public CatalogController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("test-post")]
    public IActionResult TestPost([FromBody] Product product)
    {
        _productService.AddProduct(product);
        return Ok(new { message = "Post request received" });
    }

    [HttpGet("test-connection")]
    public async Task<IActionResult> TestConnection()
    {
        var response = await _productService.PingAsync();
        if (response)
        {
            return Ok(new { message = "Elasticsearch is up and running" });
        }
        else
        {
            return StatusCode(503, new { message = "Elasticsearch is not available" });
        }
    }
    
    [HttpGet]
    [Route("count")]
    public async Task<IActionResult> GetProductCount()
    {
        var count = await _productService.GetProductCountAsync();
        return Ok(new { count });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sort
        )
    {
        var products = await _productService.GetProductsAsync(page, pageSize, search, sort);
        
        // Make products into a list of ProductDto
        return Ok(products);
    }
    
}