
//namespace TesteOAuth2Basico.Controllers
//{
//    public class Startup
//    {
//        public IConfiguration Configuration { get; }
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }
//        public void ConfigureServices(IServiceCollection services)
//        {


//            var allowedOrigins = Configuration["GOOGLE_REDIRECT_URI"];
//            services.AddCors(options =>
//            {
//                options.AddPolicy("AllowSpecificOrigin",
//                    builder => builder.WithOrigins(allowedOrigins)
//                                      .AllowAnyHeader()
//                                      .AllowAnyMethod());
//            });
//           }
//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseExceptionHandler("/Home/Error");
//                app.UseHsts();
//            }
//        }
//    }
//}
