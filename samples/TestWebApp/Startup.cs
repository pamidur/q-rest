using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using ODataSamples.Contexts;
using QRest.Core;
using System;

namespace TestWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<DataContext>(o => o.UseInMemoryDatabase("TestDb"))
                .AddQRest()
                .UseODataSemantics()
                //.UseNativeSemantics()
                .UseStandardCompiler(cpl =>
                {
                    cpl.UseCompilerCache = true;
                });

            //services
            //    .AddJsonOptions(options => { 
            //        //options.JsonSerializerOptions.ContractResolver = new DefaultContractResolver();
            //        //options.JsonSerializerOptions.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //    });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"{context.Request.Path}{context.Request.QueryString}");
                await next.Invoke();
            });

            app.UseRouting();


            app.UseODataMetadata();
            app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        }
    }
}
