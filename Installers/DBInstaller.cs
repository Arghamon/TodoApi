using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TodoApi.Services;
using TodoApi.Data;

namespace TodoApi.Installers
{
    public class DBInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("TweetBook")));
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IIdentityService, IdentityService>();
        }
    }
}
