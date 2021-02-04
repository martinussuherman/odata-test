using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ODataTest
{
    /// <summary>
    /// Represents the startup process for the application.
    /// </summary>
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
            ConfigureMisc(services);
            ConfigureCors(services);
            ConfigureOData(services);

            ApiSecurityOptions apiSecurityOptions = ReadApiSecurityOptions();
            ConfigureSwagger(services, apiSecurityOptions);
            ConfigureAuth(services, apiSecurityOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            VersionedODataModelBuilder modelBuilder,
            IApiVersionDescriptionProvider provider)
        {
            app.UseRouting();
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.Count();
                    endpoints.MapVersionedODataRoute("odata", "api", modelBuilder);
                });

            ConfigureSwaggerUI(app, provider);
        }

        private ApiSecurityOptions ReadApiSecurityOptions()
        {
            IConfigurationSection options = Configuration.GetSection(ApiSecurityOptions.OptionsName);

            return new ApiSecurityOptions
            {
                Audience = options.GetValue<string>(nameof(ApiSecurityOptions.Audience)),
                Authority = options.GetValue<string>(nameof(ApiSecurityOptions.Authority))
            };
        }
        private void ConfigureMisc(IServiceCollection services)
        {
            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.Configure<ApiSecurityOptions>(Configuration.GetSection(ApiSecurityOptions.OptionsName));
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddApiVersioning(options => options.ReportApiVersions = true);
        }
        private void ConfigureSwaggerUI(
            IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.OAuthClientId(Configuration.GetValue<string>("ClientId"));
                    options.OAuthAppName("OData Test Api Swagger");
                    options.OAuthUsePkce();

                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });
        }
        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(
                            "http://localhost:*",
                            "https://localhost:*");
                    });
            });
        }
        private static void ConfigureOData(IServiceCollection services)
        {
            services.AddOData().EnableApiVersioning();
            services.AddODataApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
        }
        private static void ConfigureSwagger(
            IServiceCollection services,
            ApiSecurityOptions apiSecurityOptions)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(
                options =>
                {
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();
                    options.OperationFilter<AuthorizeCheckOperationFilter>();

                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath);

                    options.AddSecurityDefinition(
                        ApiInfo.SchemeOauth2,
                        ConfigureSecurityDefinitionScheme(apiSecurityOptions));
                });
        }
        private static void ConfigureAuth(
            IServiceCollection services,
            ApiSecurityOptions apiSecurityOptions)
        {
            // https://identityserver4.readthedocs.io/en/latest/topics/apis.html
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // base-address of your identityserver
                    options.Authority = apiSecurityOptions.Authority;

                    // if you are using API resources, you can specify the name here
                    options.Audience = apiSecurityOptions.Audience;
                });
        }
        private static OpenApiSecurityScheme ConfigureSecurityDefinitionScheme(
            ApiSecurityOptions apiSecurityOptions)
        {
            OpenApiOAuthFlow authCodeFlow = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{apiSecurityOptions.Authority}/connect/authorize"),
                TokenUrl = new Uri($"{apiSecurityOptions.Authority}/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { apiSecurityOptions.Audience, "Api access" }
                }
            };

            return new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = authCodeFlow
                }
            };
        }

        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
