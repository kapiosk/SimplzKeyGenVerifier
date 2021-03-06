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

if (builder.Configuration.GetValue<bool>("JwtOptions:IsEnabled"))
{
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
    builder.Services.AddScoped<IJwtHandler, JwtService>();
}
else if (builder.Configuration.GetValue<bool>("JwtAsymmetricOptions:IsEnabled"))
{
    builder.Services.Configure<JwtAsymmetricOptions>(builder.Configuration.GetSection("JwtAsymmetricOptions"));
    builder.Services.AddScoped<IJwtHandler, JwtAsymmetricService>();
}


await using WebApplication app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new("en-GB")
});

app.UseResponseCompression();

app.MapGet("/Ping", SimplzKeyGenVerifier.Endpoints.Ping);
app.MapGet("/Test", SimplzKeyGenVerifier.Endpoints.Test);
app.MapPost("/RequestKey/{licenceCode}", SimplzKeyGenVerifier.Endpoints.RequestKeyAsync);

await app.RunAsync();