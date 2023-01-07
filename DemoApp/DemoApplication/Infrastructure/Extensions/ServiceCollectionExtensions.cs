using DemoApplication.Areas.Client.ActionFilter;
using DemoApplication.Database;
using DemoApplication.Infrastructure.Configurations;
using DemoApplication.Options;
using DemoApplication.Services.Abstracts;
using DemoApplication.Services.Concretes;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DemoApplication.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=>
            {
                o.Cookie.Name = "Identity";
                o.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                o.LoginPath = "/authentication/login";
                o.AccessDeniedPath = "/admin/auth/login";
            });

            services.AddHttpContextAccessor();

            services.AddScoped<ValidationCurrentUserAttribute>();

            services.ConfigureMvc();
            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


            services.ConfigureDatabase(configuration);

            services.ConfigureOptions(configuration);

            services.ConfigureFluentValidatios(configuration);

            services.RegisterCustomServices(configuration);
        }
    }
}
