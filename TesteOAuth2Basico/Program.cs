using Microsoft.Extensions.Options;
using Model;
using Services;
using StackExchange.Redis;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));

var configuration = builder.Configuration;
var googleOAuthConfig = configuration.GetSection("GoogleOAuth");

var clientId = googleOAuthConfig["ClientId"];
var clientSecret = googleOAuthConfig["ClientSecret"];
var tokenEndpoint = googleOAuthConfig["TokenEndpoint"];
var apiEndpoint = googleOAuthConfig["ApiEndpoint"];
var redirectUri = googleOAuthConfig["RedirectUri"];

var redisConfiguration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));

builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOauthSettings"));
builder.Services.AddTransient<GoogleOauthClient>(provider =>
{
    var googleOauthSettings = provider.GetRequiredService<IOptions<GoogleOAuthSettings>>().Value;
    return new GoogleOauthClient(
        googleOauthSettings.ClientId,
        googleOauthSettings.ClientSecret,
        googleOauthSettings.TokenEndpoint,
        googleOauthSettings.ApiEndpoint
    );
});

builder.Services.AddHttpClient<GoogleOauthClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:8081")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
