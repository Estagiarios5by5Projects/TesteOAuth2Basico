using Cache;
using CrossCutting.Configuration;
using dotenv.net;
using Model;
using StackExchange.Redis;
using TesteOAuth2Basico.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

new Bootstrapper(builder.Services);

// Registro das vari�veis de ambiente e configura��o do OAuth
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

// Registrar HttpClient no cont�iner de servi�os
builder.Services.AddHttpClient();

// Configura��o do Google OAuth
builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));

// Configura��o do Redis
var redisConfiguration = builder.Configuration.GetSection(CacheSettings.RedisDataSettings.ConnectionStringRedis).Value;
var redisConnectionString = Environment.GetEnvironmentVariable(CacheSettings.RedisDataSettings.ConnectionStringRedis);
if (!string.IsNullOrEmpty(redisConnectionString))
{
    redisConfiguration = redisConnectionString;
}
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));
builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

// Configura��o do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost8081",//AppSettings.CorsDataSettings.AllowedOrigins
        builder => builder.WithOrigins(AppSettings.CorsDataSettings.AllowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica a pol�tica de CORS
app.UseCors("AllowLocalhost8081");//AppSettings.CorsDataSettings.AllowedOrigins

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
