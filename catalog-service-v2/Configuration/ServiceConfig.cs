using catalog_service_2.Services;

namespace catalog_service_2.Configuration;

public class ServiceConfig
{
    public static void Configure(IServiceCollection services)
    {
        // Services
        services.AddScoped<IProductService, ProductService>();
    }

}