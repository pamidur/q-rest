using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODataSamples.Contexts;

namespace ODataSamples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseCS")));

            services
               .AddQRest()
               .UseODataSemantics()
               .UseStandardCompiler(cpl =>
               {
                   cpl.UseCompilerCache = false;
               });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var resolver = services.BuildServiceProvider();
            var ctx = resolver.GetService<DataContext>();
            ctx.Database.Migrate();
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
