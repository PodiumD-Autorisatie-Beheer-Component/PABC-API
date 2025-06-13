using PABC.Data;
using PABC.Server.Auth;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PabcDbContext>(connectionName: "Pabc");

builder.Services.AddRequestTimeouts();
builder.Services.AddOutputCache();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PodiumD Autorisatie Beheer Component API",
        Version = "v1",
        Description = "API for the PodiumD Autorisatie Beheer Component (PABC)"
    });
    options.SupportNonNullableReferenceTypes();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApiKeyAuth(builder.Configuration.GetSection("API_KEY")
    .AsEnumerable()
    .Select(x => x.Value)
    .OfType<string>()
    .ToArray());

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSwagger( x=> x.RouteTemplate = "api/{documentName}/specs.json"); //documetnname = v1

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/api/v1/specs.json", "PABC API specs");
        x.RoutePrefix = "swagger";

    });
}

app.UseAuthorization();

app.MapControllers();

app.UseRequestTimeouts();
app.UseOutputCache();

app.Run();