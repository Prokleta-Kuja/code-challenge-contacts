using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublicContacts.Api.Filters;
using PublicContacts.Api.Hubs;
using PublicContacts.App.Contexts;
using PublicContacts.Persistance.Contexts;
using Serilog;

namespace PublicContacts.Api
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
            ValidatorOptions.LanguageManager.Enabled = false;

            services
                .AddOpenApiDocument()
                .AddDbContext<IAppDbContext, AppDbContext>(o =>
                {
                    var connectionString = Configuration.GetValue<string>("ConnectionString");

                    if (string.IsNullOrWhiteSpace(connectionString))
                        o.UseSqlite("Data Source=app.db");
                    else
                        o.UseNpgsql(connectionString);
                })
                .AddMediatR(typeof(Startup).Assembly, typeof(App.Contexts.IAppDbContext).Assembly)
                .AddSpaStaticFiles(c => c.RootPath = "SPA");

            services.AddSignalR();

            services
                .AddControllers(o => o.Filters.Add(new HttpResponseExceptionFilter()))
                .AddNewtonsoftJson()
                .AddFluentValidation(f =>
                {
                    f.RegisterValidatorsFromAssembly(typeof(App.Validators.ValidatorExtensions).Assembly);
                    f.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();

            app.UseSpaStaticFiles();

            if (env.IsDevelopment())
                app.UseOpenApi(o => o.Path = "/.well-known/open-api");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ContactEventsHub>("/contacts/events");
            });

            app.UseSpa(c =>
            {
                if (env.IsDevelopment())
                    c.UseProxyToSpaDevelopmentServer("http://localhost:3000");
            });
        }
    }
}
