using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JWTBackendAuth.Models;
using JWTBackendAuth.Repository;

namespace JWTBackendAuth.Utilities
{
    public static class ConfigurationHelper
    {
#pragma warning disable CS8618
        public static IConfiguration Configuration;
#pragma warning restore CS8618 

        public static void Initialize(IConfiguration config)
        {
            Configuration = config;
        }

        public static void ConfigureAuthenticationServices(IServiceCollection services)
        {
            // Setup the DbContext to MySql
            var connetionString = Configuration.GetConnectionString("MySqlDatabase");
            services.AddDbContext<AppDbContext>(options => options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString))); ;

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // On production add more secured options
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 5;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // Add authentication to use JWT and OnAuthenticationFailed event (which is used for refreshing the tokens)
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]!);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // on production make it true
                    ValidateAudience = false, // on production make it true
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ClockSkew = TimeSpan.Zero
                };
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // Provide a header that notifies client if the JWT has expired
                        // Front end checks for this in the header and does the refresh token api
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Services that manages JWT and UserServices
            services.AddSingleton<IJWTManagerRepository, JWTManagerRepository>();
            services.AddScoped<IUserServiceRepository, UserServiceRepository>();
            services.AddScoped<IValidator<string>, EmailValidator>();
            services.AddControllers();
        }
    }
}
