using catalog_service_v2.Configuration;
using Microsoft.OpenApi.Models;

namespace catalog_service_v2;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    private IConfiguration Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services)
    {
        /* Gets the necessary configuration values, if run locally gets local values, README.md for setting those values */
        var elasticSearchUserName = "elastic";
        var elasticSearchPassword = "changeme";
        
        // Configurations for the services
        ElasticSearchConfig.Configure(services, elasticSearchUserName, elasticSearchPassword);
        ServiceConfig.Configure(services);
        
        // Add controllers to the service, this is where the API endpoints are defined
        services.AddControllers();
        
        // Adds cors policy which allows any origin, method and header
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        // Setup Swagger for API documentation
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Migrate database to latest version if necessary
        app.UseCors("AllowAll");
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
            Console.WriteLine("Swagger setup complete.");
        }
        else
        {
            Console.WriteLine("Not in development mode, skipping Swagger setup.");
        }
        
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}