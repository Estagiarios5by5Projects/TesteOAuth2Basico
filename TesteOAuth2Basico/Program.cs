using Cache;
using CrossCutting.Configuration;
using dotenv.net;
using Model;
using StackExchange.Redis;
using TesteOAuth2Basico.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

new Bootstrapper(builder.Services);

// Registro das variáveis de ambiente e configuração do OAuth
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

// Registrar HttpClient no contêiner de serviços
builder.Services.AddHttpClient();

// Configuração do Google OAuth
builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));

// Configuração do Redis
var redisConfiguration = builder.Configuration.GetSection(CacheSettings.RedisDataSettings.ConnectionStringRedis).Value;
var redisConnectionString = Environment.GetEnvironmentVariable(CacheSettings.RedisDataSettings.ConnectionStringRedis);
if (!string.IsNullOrEmpty(redisConnectionString))
{
    redisConfiguration = redisConnectionString;
}
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));
builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

// Configuração do CORS
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

// Aplica a política de CORS
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
