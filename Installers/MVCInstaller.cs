using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;

using TodoApi.Options;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace TodoApi.Installers
{
    public class MVCInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllers();

            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(JwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                //ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters); 

            services.AddAuthentication(options =>
           {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
           })
           .AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.TokenValidationParameters = tokenValidationParameters;
           });

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "TweetBook Api", Version = "v1" });

                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[0] } };

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme{Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                },
                        new List<string>()}
                });

            });
        }
    }
}
