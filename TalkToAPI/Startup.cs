using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TalkToAPI.Database;
using TalkToAPI.Helpers;
using TalkToAPI.Helpers.Swagger;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI
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
            // Config Method
            #region Configure Method
            services.Configure<ApiBehaviorOptions>(cfg => {
                cfg.SuppressModelStateInvalidFilter = true;
            });
            #endregion

            // AddMvc - API - Config
            #region Add Mvc - Config
            services.AddMvc();

            #endregion

            // Api Versioning
            #region API Versioning - Config
            services.AddApiVersioning(cfg => {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
            });
            #endregion

            // Database - Config
            #region DB - Config
            services.AddDbContext<TalkToContext>(cfg => {
                cfg.UseSqlite("Data Source=Database\\TalkTo.db");
            });
            #endregion

            //  Dependencies Repos
            #region Dependency Inject - Config
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            #endregion

            //  Identity - Config
            #region Identity - Config
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<TalkToContext>()
                .AddDefaultTokenProviders();
            #endregion

            // Authentication - Config
            #region Authentication & JWT - Config
            services.AddAuthentication(cfg => {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg => {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key-api-jwt-to-do-application"))
                };
            });
            #endregion

            //Authorization - Config
            #region Authorization - Config
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                            .RequireAuthenticatedUser()
                                            .Build()
                );
            });

            services.ConfigureApplicationCookie(cfg => {
                cfg.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
            #endregion

            //Add Controllers - Config
            #region Add Controllers - Config
            services.AddControllers(cfg =>
            {
                cfg.RespectBrowserAcceptHeader = false;
                cfg.ReturnHttpNotAcceptable = true;
/*                cfg.InputFormatters.Add(new XmlSerializerInputFormatter(cfg));
                cfg.OutputFormatters.Add(new XmlSerializerOutputFormatter());*/
            }).AddNewtonsoftJson().AddXmlSerializerFormatters();
            #endregion

            //Auto Mapper - Config
            #region Auto-Mapper - Config 
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            //Swagger - Config
            #region  Swagger Doc & Api Swagger Versioning - Config  
            services.AddSwaggerGen(cfg =>
            {
                cfg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = (ParameterLocation)1,
                    Type = 0,
                    Description = "Adicione o JSON WEB TOKEN para autenticar.",
                    Name = "Authorization"
                });

                cfg.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer", //The name of the previously defined security scheme.
                            Type = ReferenceType.SecurityScheme
                        }
                    },new List<string>()
                }
            });

                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
                cfg.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
                cfg.SwaggerDoc("v1", new OpenApiInfo { Title = "TalkToAPI - v1.0", Version = "v1" });

                var projectPath = PlatformServices.Default.Application.ApplicationBasePath;
                var projectName = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var pathXMLFileDoc = Path.Combine(projectPath, projectName);

                cfg.IncludeXmlComments(pathXMLFileDoc);

                cfg.DocInclusionPredicate((docName, apiDesc) => {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<FilterSwagger>();
            });


            services.AddApiVersioning(cfg => {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Env Config
            #region Development Env - Config
            if (env.IsDevelopment())
            {
            }
            #endregion

            // Use APP - Config
            #region Use APP - Config

            
            app.UseHttpsRedirection();
            app.UseStatusCodePages();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();



            #endregion

            // Endpoints - Config
            #region Endpoints APP - Config
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #endregion

            // Swagger APP config
            #region Swagger App - Config  
            app.UseSwagger(); // /swagger/v1/swagger.json
            app.UseSwaggerUI(cfg => {
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "TalkToAPI v1");
            });
            #endregion

        }
    }
}
