using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Application.Interfaces;
using Application.Profiles;
using Application.Values;
using AutoMapper;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Errors;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Swashbuckle.AspNetCore.Swagger;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsDevelopment())
            {
                services.AddDbContext<DataContext>(opt =>
                {
                    opt.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
                });
            }
            else
            {
                services.AddDbContext<DataContext>(opt =>
                {
                    opt.UseSqlServer("Server=fake;Initial Catalog=activityhubdb;Persist Security Info=False;User ID=fakeuser;Password=fakepassword;");
                });
            }
           
            var builder = services.AddIdentityCore<AppUser>();
            builder = new IdentityBuilder(builder.UserType, builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddSignInManager<SignInManager<AppUser>>();

            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IProfileReader, ProfileReader>();
            services.AddScoped<ICloudinaryAccessor, CloudinaryAccessor>();
            services.Configure<CloudinarySettings>(_configuration.GetSection("Cloudinary"));
            
            services.AddMediatR(typeof(List.Handler).Assembly);
            services.AddAutoMapper();
            
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field test",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] {}}
                });
                c.SwaggerDoc("v1", new Info { Title = "ActivityHub API", Version = "v1" });
                c.CustomSchemaIds(x => x.FullName);
                
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("IsActivityHost", policy => { policy.Requirements.Add(new IsHostRequirement()); });
            });
            services.AddTransient<IAuthorizationHandler, IsHostHandler>();
            
            services.AddMvc(opt =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    opt.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFluentValidation(cfg =>
                {
                    cfg.RegisterValidatorsFromAssemblyContaining(typeof(Application.Values.Details));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {     
            app.UseMiddleware<ErrorHandlingMiddleware>();
            
            if (_env.IsDevelopment())
            {
                
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "Activity Hub API"); });
        }
    }
}
