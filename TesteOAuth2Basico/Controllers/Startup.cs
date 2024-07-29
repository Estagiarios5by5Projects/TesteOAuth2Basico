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
            services.AddControllers();
            //services.AddTransient<GoogleOauthClient>();
            //// Configuração Google OAuth
            //services.Configure<GoogleOAuthSettings>(Configuration.GetSection("GoogleOAuth"));

            //// Configuração do GoogleOauthClient
            //services.AddSingleton<GoogleOauthClient>(provider =>
            //{
            //    var settings = Configuration.GetSection("GoogleOAuth").Get<GoogleOAuthSettings>();
            //    return new GoogleOauthClient(settings.ClientId, settings.ClientSecret, settings.TokenEndpoint, settings.ApiEndpoint);
            //});

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

