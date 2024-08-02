using Microsoft.Extensions.Options;
using Model;
using Services;
using StackExchange.Redis;
using dotenv.net;
using TesteOAuth2Basico.Repository;
using Repositories.Utils;

var builder = WebApplication.CreateBuilder(args);

// Registro das variáveis de ambiente e configuração do OAuth
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

//builder.Services.AddScoped<CreateUserCommandHandler>();

// Registrar HttpClient no contêiner de serviços
builder.Services.AddHttpClient();

// Configuração do Google OAuth
builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));

// Configuração do Redis
var redisConfiguration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    redisConfiguration = redisConnectionString;
}
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));
builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

// Registro de serviços
builder.Services.AddTransient<GoogleOauthClient>(provider =>
{
    var googleOauthSettings = provider.GetRequiredService<IOptions<GoogleOAuthSettings>>().Value;
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new GoogleOauthClient(
        googleOauthSettings.ClientId,
        googleOauthSettings.ClientSecret,
        googleOauthSettings.TokenEndpoint,
        googleOauthSettings.ApiEndpoint,
        httpClient
    );
});

builder.Services.AddSingleton<UserRepository>();


// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost8081",
        builder => builder.WithOrigins("http://localhost:8081")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica a política de CORS
app.UseCors("AllowLocalhost8081");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
