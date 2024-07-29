using Microsoft.Extensions.Options;
using Model;
using Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
