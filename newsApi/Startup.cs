using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using newsApi.Data;
using newsApi.Models;

namespace newsApi
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins(
                                "http://localhost:3000",
                                "https://localhost:3000",
                                "https://bkalafat.github.io/",
                                "https://haberibul.com",
                                "http://haberibul.com",
                                "https://www.haberibul.com",
                                "https://news-livid.vercel.app",
                                "https://news-git-master.bkalafat.vercel.app",
                                "https://news-git-master.bkalafat.vercel.app",
                                "https://news-4znxe32rk.vercel.app",
                                "https://news.bkalafat.vercel.app",
                                "https://news-git-develop.bkalafat.vercel.app",
                                "https://news-n8bzqvwmp.vercel.app",
                                "http://www.haberibul.com",
                                "http://m.haberibul.com",
                                "https://m.haberibul.com",
                                "http://localhost:8000",
                                "https://news-26417.web.app",
                                "https://haberibul.web.app",
                                "https://haberibul.firebaseapp.com")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            services.Configure<NewsDatabaseSettings>(Configuration.GetSection(nameof(NewsDatabaseSettings)));

            services.AddSingleton<INewsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<NewsDatabaseSettings>>().Value);

            services.AddControllers();
            services.AddMemoryCache();
            services.AddScoped<INewsService, NewsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}