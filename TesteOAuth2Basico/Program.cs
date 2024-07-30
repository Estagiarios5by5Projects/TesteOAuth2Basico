using Microsoft.Extensions.Options;
using Model;
using Services;
using StackExchange.Redis;
using dotenv.net;
using TesteOAuth2Basico.Repository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();//carrega variáveis do .env
builder.Configuration.AddEnvironmentVariables();//adiciona as variáveis de ambiente

builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));//configuração do google oauth
builder.Services.AddSingleton<UserRepository>();//adiciona o serviço de autorização do google

var configuration = builder.Configuration;
var redisConfiguration = builder.Configuration.GetSection("Redis:ConnectionString").Value;//string de conexão do redis
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");//obtém a string de conexão do redis da variável de ambiente
if (!string.IsNullOrEmpty(redisConnectionString))
{
    redisConfiguration = redisConnectionString;
}
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));//adiciona a conexão do redis

builder.Services.AddTransient<GoogleOauthClient>(provider =>//adiciona o cliente do google oauth
{
    var googleOauthSettings = provider.GetRequiredService<IOptions<GoogleOAuthSettings>>().Value;//obtém as configurações do google oauth
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
