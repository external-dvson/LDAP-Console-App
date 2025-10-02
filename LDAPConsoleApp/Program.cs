using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using LDAPConsoleApp.Configuration;
using LDAPConsoleApp.Interfaces;
using LDAPConsoleApp.Helpers;

namespace LDAPConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var settings = serviceProvider.GetRequiredService<IOptions<LdapSettings>>();

            DisplayHelper.DisplayApplicationHeader(
                settings.Value.Domain,
                Environment.UserDomainName,
                Environment.UserName
            );

            var testRunner = serviceProvider.GetRequiredService<LDAPTest>();
            testRunner.RunTest();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(CommonConstant.Configuration.AppSettingsFile, optional: false, reloadOnChange: true)
                .Build();

            services.Configure<LdapSettings>(configuration.GetSection(CommonConstant.Configuration.LdapSettingsSection));
            services.AddScoped<ILdapService, LDAPService>();
            services.AddScoped<LDAPTest>();
        }
    }
}
