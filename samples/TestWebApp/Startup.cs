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
                //.UseODataSemantics()
                .UseNativeSemantics()
                .UseStandardCompiler(cpl =>
                {
                    cpl.UseCompilerCache = false;
                });

            services
                .AddMvc()
                .AddJsonOptions(options => { 
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"{context.Request.Path}{context.Request.QueryString}");
                await next.Invoke();
            });


            app.UseODataMetadata();
            app.UseMvc();
        }
    }
}
