using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using System.Text;
using Repositories.Utils;
using Services;
using StackExchange.Redis;
using TesteOAuth2Basico.Repository;

namespace TesteOAuth2Basico.Controllers
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var allowedOrigins = Configuration["GOOGLE_REDIRECT_URI"];
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins(allowedOrigins)
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });
            
            //services.AddScoped<GetUserByIdQueryHandler>();
            services.AddScoped<UserRepository>();
            services.AddScoped<ILogger<UserController>, Logger<UserController>>();

            // Registrar HttpClient
            services.AddHttpClient();

            // Configuração do Google OAuth
            services.Configure<GoogleOAuthSettings>(Configuration.GetSection("GoogleOauthSettings"));
            services.AddTransient<GoogleOauthClient>(provider =>
            {
                var googleOauthSettings = provider.GetRequiredService<IOptions<GoogleOAuthSettings>>().Value;
                var httpClient = provider.GetRequiredService<HttpClient>();
                return new GoogleOauthClient(
                    googleOauthSettings.ClientId,
                    googleOauthSettings.ClientSecret,
                    googleOauthSettings.TokenEndpoint,
                    googleOauthSettings.ApiEndpoint,
                    httpClient // Passa o HttpClient para o construtor
                );
            });

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

            // Configuração do JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]))
                    };
                });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
