using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TodoApi.Services;
using TodoApi.Data;

namespace TodoApi.Installers
{
    public class DBInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("TweetBook")));
            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<DataContext>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IIdentityService, IdentityService>();
        }
    }
}
