using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
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
            //services.AddOData("/api");

            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                //.AddQRestOptions(qrest =>
                //{
                //    qrest.Semantics = new QRestSemantics { UseDefferedConstantParsing = DefferedConstantParsing.StringsAndNumbers };
                //    qrest.Compiler = new StandardCompiler { UseCompilerCache = false };
                //})
                ;
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


            //app.UseODataMetadata();
            app.UseMvc();
        }
    }
}
