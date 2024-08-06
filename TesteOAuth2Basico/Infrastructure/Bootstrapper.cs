using Cache;
using CrossCutting.Configuration;
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
using TesteOAuth2Basico.Repository;

namespace TesteOAuth2Basico.Infrastructure
{
    public class Bootstrapper
    {
        //todas as injeções de dependência

        public Bootstrapper(IServiceCollection services)
        {

        }
        private static void InjectionMediator(IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }

        private static void InjectionScooped(IServiceCollection services)
        {
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
        }

        private static void InjectionTransient(IServiceCollection services)
        {
            services.AddTransient<GoogleOauthClient>(provider =>
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
        }
        private static void InjectionAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = AppSettings.JWTDataSettings.Issuer,
                        ValidAudience = AppSettings.JWTDataSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JWTDataSettings.SecretKey))
                    };
                });
        }
        private static void InjectionCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins(AppSettings.OAuthDataSettings.RedirectUri) //ERRADO CORS
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });
        }
        private static void InjectionRedis(IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(CacheSettings.RedisDataSettings.ConnectionStringRedis));
            services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        }
        private static void InjectionHttpClient(IServiceCollection services)
        {
            services.AddHttpClient();
        }
        private static void InjectionLogger(IServiceCollection services)
        {
            services.AddScoped<ILogger<UserController>, Logger<UserController>>();
        }
    }
}
