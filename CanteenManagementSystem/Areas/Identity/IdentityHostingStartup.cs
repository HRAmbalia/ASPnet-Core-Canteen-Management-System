using System;
using CanteenManagementSystem.Areas.Identity.Data;
using CanteenManagementSystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CanteenManagementSystem.Areas.Identity.IdentityHostingStartup))]
namespace CanteenManagementSystem.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<CmsDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CmsDbContextConnection")));

                services.AddDefaultIdentity<CmsAppUser>(options => {
                    options.SignIn.RequireConfirmedEmail = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = true;
                }).AddEntityFrameworkStores<CmsDbContext>().AddDefaultTokenProviders();
            });
        }
    }
}