using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using app.Services;

namespace app
{
    public class Program
    {
        private const string serviceBusConnectionString = "";
        private const string queueName = "hike-tracker-queue";

        public static void Main(string[] args)
        {
            HikeQueue.Initialize(serviceBusConnectionString, queueName);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
