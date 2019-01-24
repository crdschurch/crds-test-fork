using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        var swaggerInfo = new Swashbuckle.AspNetCore.Swagger.Info
        {
            Title = "Test",
            Description = "Desc",
            Version = "1",
            License = new Swashbuckle.AspNetCore.Swagger.License { Name = "Use under LICX", Url = "https://example.com/license" },
            TermsOfService = ""
        };

        services.AddSwaggerGen(c => c.SwaggerDoc("", swaggerInfo));
    }
}