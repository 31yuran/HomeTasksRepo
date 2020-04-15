using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTaskApi.Controllers;
using HomeTaskApi.Models;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeTaskApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var con = "Server=(localdb)\\mssqllocaldb;Database=tasksdbstore;Trusted_Connection=True;";
            services.AddDbContext<HomeTasksContext>(options => options.UseSqlServer(con)); // устанавливаем контекст данных
            services.AddControllers().AddNewtonsoftJson(
                options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); // используем контроллеры без представлений
            services.AddCors();
            /*services.AddCors(options =>
            {
                options.AddPolicy("AllowTestSite", builder => builder
                    .WithOrigins("https://localhost:56964")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });*/
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            // подключаем CORS
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<MyHub>("/myHub");
            }) ;
        }
    }
}
