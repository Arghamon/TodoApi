using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TodoApi.Installers
{
    public class TestService : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}
