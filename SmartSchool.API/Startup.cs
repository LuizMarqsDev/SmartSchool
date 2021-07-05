using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartSchool.API.Data;

namespace SmartSchool.API
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
            services.AddDbContext<SmartContext>(
               context => context.UseMySql(Configuration.GetConnectionString("MySqlConnection"))
            );

            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling =
                                                                Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddScoped<IRepository, Repository>();

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            var apiProvidaderDescription = services.BuildServiceProvider()
                                                   .GetService<IApiVersionDescriptionProvider>();
             
            services.AddSwaggerGen(options => 
            {
                foreach (var description in apiProvidaderDescription.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                   description.GroupName,
                   new Microsoft.OpenApi.Models.OpenApiInfo()
                   {
                       Title = "SmartSchool API",
                       Version = description.ApiVersion.ToString(),
                       Description = " API de estudo do curso de WebAPI RESTful ",
                       TermsOfService = new Uri(" https://example.com/terms "),
                       License = new Microsoft.OpenApi.Models.OpenApiLicense
                       {
                           Name = "SmartSchool License",
                           Url = new Uri("http://mit.com")
                       },
                       Contact = new Microsoft.OpenApi.Models.OpenApiContact
                       {
                           Name = " Luiz Marques ",
                           Email = " marqsuiz@gmail.com ",
                           Url = new Uri(" https://www.linkedin.com/in/luiz-marques-447729162/ "),
                       }
                   });
                }

                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);

                options.IncludeXmlComments(xmlCommentFullPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiProvidaderDescription)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger()
                .UseSwaggerUI(options => {

                    foreach (var description in apiProvidaderDescription.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                   
                    options.RoutePrefix = "";
                });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
