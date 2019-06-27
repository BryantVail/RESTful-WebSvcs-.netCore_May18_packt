using DemoECommerceApp.Interfaces;
using DemoECommerceApp.Models;
using DemoECommerceApp.OAuth;
using DemoECommerceApp.Security.Authentication;
//using DemoECommerceApp.Data;
using DemoECommerceApp.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoECommerceApp
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
            services.AddSingleton<IProductService, ProductService>();
            services.AddMvc();

            //Authentication Service
            services.AddAuthentication("Basic")
                .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);
            services.AddTransient<IAuthenticationHandler, BasicAuthenticationHandler>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = "http://localhost:57571";
                o.Audience = "FlixOneStore.ReadAccess";
                o.RequireHttpsMetadata = false;
            });



            //IdentityServer
            services.AddIdentityServer()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();

            //db connection
            var connection = @"Server=localhost\SQLExpress;Database=FlixOneStore;Trusted_Connection=True";
            services.AddDbContext<FlixOneStoreContext>(
                options => options.UseSqlServer(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
