
using Microsoft.Extensions.Options;
using Model;
using Services;
using StackExchange.Redis;

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
            services.AddOptions();
            services.Configure<GoogleOAuthSettings>(Configuration.GetSection("GoogleOauthSettings"));
            services.AddTransient<GoogleOauthClient>(provider =>
            {
                var googleOauthSettings = provider.GetRequiredService<IOptions<GoogleOAuthSettings>>().Value;
                return new GoogleOauthClient(
                    googleOauthSettings.ClientId,
                    googleOauthSettings.ClientSecret,
                    googleOauthSettings.TokenEndpoint,
                    googleOauthSettings.ApiEndpoint
                );
            });

            // Configuração do Redis
            var redisConfiguration = Configuration.GetSection("Redis:ConnectionString").Value;
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));
            services.AddControllers();
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

