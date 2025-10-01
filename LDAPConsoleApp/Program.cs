using Microsoft.Extensions.Configuration;

namespace LDAPConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var domain = configuration["LdapSettings:Domain"] ?? "APAC.bosch.com";

            Console.WriteLine(" LDAP Console Application với Windows Authentication");
            Console.WriteLine($"   Domain: {domain}");
            Console.WriteLine($"   Current User: {Environment.UserDomainName}\\{Environment.UserName}");
            Console.WriteLine();

            LDAPTest.RunTest();
        }
    }
}
