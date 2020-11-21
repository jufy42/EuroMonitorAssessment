using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Autofac.Extensions.DependencyInjection;

namespace Library___ASP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder         
                        .UseStartup<Startup>()
                        .ConfigureLogging((hostingContext, builder) => {
                            builder.ClearProviders();
                            builder.SetMinimumLevel(LogLevel.Trace);
                            builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                            builder.AddConsole();
                            builder.AddDebug();
                            builder.AddFile("Library.log");
                        });
                });
    }
}
