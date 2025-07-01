using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PABC.Server.Config
{
    public record ApiOptions
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }

    public static class ApiVersioningExtensions
    {
        public static void ConfigureApiVersioningWithOpenApi(this IServiceCollection services, ApiOptions apiVersionOptions)
        {
            services.Configure<ApiOptions>(o =>
            {
                o.Description = apiVersionOptions.Description;
                o.Title = apiVersionOptions.Title;
            });

            services.Configure<ApiVersioningOptions>(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.Configure<ApiExplorerOptions>(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUIOptions>();
            services.ConfigureSwagger(opt =>
            {
                opt.RouteTemplate = "api/{documentName}/specs.json";
            });
            services.Configure<MvcOptions>(options => options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider()));
        }

        public static void MapVersionedControllers(this IEndpointRouteBuilder builder) =>
            builder.MapGroup("/api/v{v:apiVersion}").MapControllers();

        private class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider, IOptions<ApiOptions> apiOptions) : IConfigureOptions<SwaggerGenOptions>
        {
            public void Configure(SwaggerGenOptions options)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        description.GroupName,
                        new OpenApiInfo()
                        {
                            Title = apiOptions.Value.Title,
                            Description = apiOptions.Value.Title,
                            Version = description.GroupName,
                        });
                }
                //options.OperationFilter<SwaggerDefaultValues>();
                options.DocumentFilter<SwaggerServersFilter>();
            }
        }

        private class SwaggerServersFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument doc, DocumentFilterContext context)
            {
                var path = "/api/" + doc.Info.Version;
                doc.Servers.Add(new OpenApiServer() { Url = path });
            }
        }

        private class ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerUIOptions>
        {
            public void Configure(SwaggerUIOptions options)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/api/{description.GroupName}/specs.json",
                        description.GroupName);
                }
            }
        }
    }
}
