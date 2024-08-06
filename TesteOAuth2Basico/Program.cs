using CrossCutting.Configuration;
using Domain.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using Repositories.Utils;
using Services;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using TesteOAuth2Basico.Controllers;
using TesteOAuth2Basico.Infrastructure;
using TesteOAuth2Basico.Repository;

var builder = WebApplication.CreateBuilder(args);

new Bootstrapper(builder.Services);

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
//builder.Services.AddMediatR(typeof(CreateUserCommandHandler).Assembly);
//builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuerJwt = AppSettings.JWTDataSettings.Issuer;
        var audienceJwt = AppSettings.JWTDataSettings.Audience;
        var secretKeyJwt = AppSettings.JWTDataSettings.SecretKey;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuerJwt,
            ValidAudience = audienceJwt,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyJwt))
        };
    });
builder.Services.AddAuthorization(); 
builder.Services.AddCors(options =>
{
    var allowedOrigins = AppSettings.CorsDataSettings.AllowedOrigins;
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});
var connectionStringRedis = AppSettings.RedisDataSettings.ConnectionStringRedis;
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionStringRedis));
builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
builder.Services.AddHttpClient();
builder.Services.AddScoped<ILogger<UserController>, Logger<UserController>>();

builder.Services.AddSwaggerGen();

// Adicione serviços ao contêiner
var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure o pipeline de solicitação HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


//// Registrar HttpClient no contêiner de serviços
//builder.Services.AddHttpClient();

//// Configuração do Google OAuth
//builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));

//// Configuração do Redis
//var redisConfiguration = builder.Configuration.GetSection(CacheSettings.RedisDataSettings.ConnectionStringRedis).Value;
//var redisConnectionString = Environment.GetEnvironmentVariable(CacheSettings.RedisDataSettings.ConnectionStringRedis);
//if (!string.IsNullOrEmpty(redisConnectionString))
//{
//    redisConfiguration = redisConnectionString;
//}
//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));
//builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

//// Configuração do CORS

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost8081",//AppSettings.CorsDataSettings.AllowedOrigins
//        builder => builder.WithOrigins(AppSettings.CorsDataSettings.AllowedOrigins)
//                          .AllowAnyHeader()
//                          .AllowAnyMethod());
//});

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//Aplica a política de CORS
//app.UseCors("AllowLocalhost8081");//AppSettings.CorsDataSettings.AllowedOrigins

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseCors(AppSettings.CorsDataSettings.AllowedOrigins);
//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();
//app.Run();
