using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using SimplzKeyGenVerifier.Data;
using SimplzKeyGenVerifier.Options;
using SimplzKeyGenVerifier.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWTOptions"));
builder.Services.AddScoped<JWTService>();

await using WebApplication app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new("en-GB")
});

app.UseResponseCompression();

app.MapGet("/Ping", SimplzKeyGenVerifier.Endpoints.Ping);
app.MapPost("/RequestKey/{licence}", SimplzKeyGenVerifier.Endpoints.RequestKeyAsync);

await app.RunAsync();