using Microsoft.Extensions.Options;
using Model;
using Services;
using StackExchange.Redis;
using dotenv.net;
using TesteOAuth2Basico.Repository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();//carrega vari�veis do .env
builder.Configuration.AddEnvironmentVariables();//adiciona as vari�veis de ambiente

builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));//configura��o do google oauth
builder.Services.AddSingleton<UserRepository>();//adiciona o servi�o de autoriza��o do google

var configuration = builder.Configuration;
var redisConfiguration = builder.Configuration.GetSection("Redis:ConnectionString").Value;//string de conex�o do redis
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");//obt�m a string de conex�o do redis da vari�vel de ambiente
if (!string.IsNullOrEmpty(redisConnectionString))
{
    redisConfiguration = redisConnectionString;
}
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));//adiciona a conex�o do redis

builder.Services.AddTransient<GoogleOauthClient>(provider =>//adiciona o cliente do google oauth
{
    var googleOauthSettings = provider.GetRequiredService<IOptions<GoogleOAuthSettings>>().Value;//obt�m as configura��es do google oauth
    return new GoogleOauthClient(
        googleOauthSettings.ClientId,
        googleOauthSettings.ClientSecret,
        googleOauthSettings.TokenEndpoint,
        googleOauthSettings.ApiEndpoint
    );
});

builder.Services.AddTransient<UserRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redis = ConnectionMultiplexer.Connect(redisConfiguration);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services.AddTransient<UserRepository>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
