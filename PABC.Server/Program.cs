using PABC.Data;
using PABC.Server.Auth;
using System.Reflection;
using PABC.Server.Config;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PabcDbContext>(connectionName: "Pabc");

builder.Services.AddRequestTimeouts();
builder.Services.AddOutputCache();
builder.Services.AddProblemDetails();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApiKeyAuth(builder.Configuration.GetSection("API_KEY")
    .AsEnumerable()
    .Select(x => x.Value)
    .OfType<string>()
    .ToArray());

builder.Services.AddApiVersioning().AddMvc().AddApiExplorer();

builder.Services.ConfigureApiVersioningWithOpenApi(new()
{
    Title = "Platform Autorisatie Beheer Component API",
    Description = "API for the Platform Autorisatie Beheer Component (PABC)"
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(x=> x.RoutePrefix = "swagger");
}
app.UseAuthorization();

app.MapVersionedControllers();

app.UseRequestTimeouts();
app.UseOutputCache();

app.Run();
