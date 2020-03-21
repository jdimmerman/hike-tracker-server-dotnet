using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    var vaultUrl = $"https://{builtConfig["VaultName"]}.vault.azure.net/";
                    Console.WriteLine($"vaultUrl {vaultUrl}");
                    var configBuilder = config.AddAzureKeyVault(vaultUrl);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
