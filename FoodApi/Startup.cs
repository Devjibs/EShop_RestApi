using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FoodApi
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
            services.AddControllers();
            services.AddMvc(option => option.EnableEndpointRouting = false)
           .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
           .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["Tokens:Issuer"],
                       ValidAudience = Configuration["Tokens:Issuer"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                       ClockSkew = TimeSpan.Zero,
                   };
               });
            services.AddDbContext<FoodDbContext>(option => option.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FoodAppDb"));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Food Order API",
                    Description = "A food order mobile app rest api",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                {
                                    {
                                          new OpenApiSecurityScheme
                                            {
                                                Reference = new OpenApiReference
                                                {
                                                    Type = ReferenceType.SecurityScheme,
                                                    Id = "Bearer"
                                                }
                                            },
                                            new string[] {}

                                    }
                                });
            });


            
            //services.AddMvcCore()
            //    .AddAuthorization()
            //    .AddApiExplorer()
            //    .AddDataAnnotations()
            //    .AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FoodDbContext foodDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Food Order API");
            });

            app.UseCors(configuration => configuration
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition"));

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            foodDbContext.Database.EnsureCreated();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        
        }
    }
}




//using CarePlus.API.Filters.SwashBuckle;
//using CarePlus.API.Middlewares;
//using CarePlus.Core.Entities;
//using CarePlus.Service.Extension;
//using CarePlus.Service.Implementation;
//using CarePlus.Service.Interface;
//using CarePlus.Service.Utilities;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using Newtonsoft.Json.Converters;
//using Newtonsoft.Json.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CarePlus.API
//{
//  public class Startup
//  {
//    public Startup(IConfiguration configuration)
//    {
//      Configuration = configuration;
//    }

//    public IConfiguration Configuration { get; }

//    // This method gets called by the runtime. Use this method to add services to the container.
//    public void ConfigureServices(IServiceCollection services)
//    {
//      //services.AddControllers();

//      services.Configure<ReadConfig>(Configuration.GetSection("ReadConfig"));
//      var  config= Configuration.GetSection("ReadConfig");
//      ReadConfig readconfig = config.Get<ReadConfig>();
//      var secretKey =new SymmetricSecurityKey(Encoding.ASCII.GetBytes(readconfig.Secret));
//      services.AddAuthentication(option =>
//      {
//        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

//      }).AddJwtBearer(options =>
//      {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;
//        options.TokenValidationParameters = new TokenValidationParameters
//        { 
//          ValidateLifetime = false,
//          ValidateIssuerSigningKey = true,
//          ValidIssuer = readconfig.Issuer, 
//          ValidAudiences = new List<string>
//        {
//            readconfig.Audience1,
//            readconfig.Audience2
//        },
//          IssuerSigningKey = secretKey 
//        };
//      });
//            services
//        .AddMvcCore()
//        .AddAuthorization()
//        .AddApiExplorer()
//        .AddDataAnnotations()
//        .AddCors()
//        .AddNewtonsoftJson(options =>
//        {
//          options.SerializerSettings.Converters.Add(new StringEnumConverter { AllowIntegerValues = true });
//          options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
//          options.SerializerSettings.ContractResolver = new DefaultContractResolver();
//          options.SerializerSettings.Error = (sender, args) =>
//          {
//            throw args?.ErrorContext?.Error;
//          };
//        })
//         .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
     
//      services.AddSwaggerGen(c =>
//      {
//        c.SwaggerDoc("v1", new OpenApiInfo
//        {
//          Title = "CarePlus API",
//          Version = "v1",
//          Description = "CarePlus Services.",
//          Contact = new OpenApiContact
//          {
//            Name = "Sidmach Technologies Nig. LTD.",
//            Email = string.Empty,
//            Url = new Uri("https://sidmach.com/"),
//          },
//        });
//        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//        {
//          Name = "Authorization",
//          Type = SecuritySchemeType.ApiKey,
//          Scheme = "Bearer",
//          BearerFormat = "JWT",
//          In = ParameterLocation.Header,
//          Description = "JWT Authorization header using the Bearer scheme."
//        });

//        c.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    {
//                          new OpenApiSecurityScheme
//                            {
//                                Reference = new OpenApiReference
//                                {
//                                    Type = ReferenceType.SecurityScheme,
//                                    Id = "Bearer"
//                                }
//                            },
//                            new string[] {}

//                    }
//                });
//        c.EnableAnnotations();
//        c.DocumentFilter<KnownTypesResponseFilter>();
//        c.OperationFilter<ClientFaultResponseFilter>();
//        c.OperationFilter<ServerFaultResponseFilter>();
//        c.OperationFilter<HttpHeadersResponseFilter>();

//      });


//      services.AddHttpContextAccessor();
//      services.AddService(Configuration);
//      services.Configure<SMTPConfigModel>(Configuration.GetSection("SMTPConfig"));
//    }

//    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//    {
//      if (env.IsDevelopment())
//      {
//        app.UseDeveloperExceptionPage();
//      }
//      app.UseCors(configuration => configuration
//                .AllowAnyOrigin()
//                .AllowAnyMethod()
//                .AllowAnyHeader()
//                .WithExposedHeaders("Content-Disposition"));

//      app.UseSwagger(c =>
//      {
//        c.SerializeAsV2 = true;
//      });

//      app.UseSwaggerUI(c =>
//      {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Care Plus API");
//      });

//      app.UseHttpsRedirection();

//      app.UseRouting();
//      app.UseAuthentication();
//      app.UseMiddleware<ExceptionHandlingMiddleware>();
//      app.UseMiddleware<JwtMiddleware>();
//      app.UseAuthorization();

//      app.UseEndpoints(endpoints =>
//      {
//        endpoints.MapControllers();
//      });
//    }
//  }
//}
