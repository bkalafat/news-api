using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using newsApi.Common;
using newsApi.Data;
using System;

namespace newsApi
{
    public class Startup
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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
                                "https://bkalafat.github.io",
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
                                "https://tskulis.vercel.app",
                                "https://tskulis.com",
                                "https://www.tskulis.com",
                                "http://m.tskulis.com",
                                "https://tskulis-bkalafat.vercel.app",
                                "https://tskulis-git-master-bkalafat.vercel.app",
                                "https://haberibul.firebaseapp.com")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            services.Configure<NewsDatabaseSettings>(Configuration.GetSection(nameof(NewsDatabaseSettings)));

            services.AddSingleton<INewsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<NewsDatabaseSettings>>().Value);

            services.AddControllers();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
            services.AddMemoryCache();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<ICommentService, CommentService>();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "News API",
                    Version = "v1",
                    Description = ".net 6 Web API for News",
                    Contact = new OpenApiContact
                    {
                        Name = "Bkalafat Software Technologies",
                        Email = "kalafatburak@gmail.com",
                        Url = new Uri("https://github.com/bkalafat"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "News API V3");
            });

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