using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shared.Data;
using Shared.MsLogger.Extensions;
using WebLogViewer.ViewModels;

namespace WebLogViewer
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
			builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			builder.Logging.AddBlazorLogger();

            builder.Services.AddSingleton<IDataStore, LogDataStore>();
            builder.Services.AddSingleton<WebLogViewModel>();

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILoggerFactory>()
                .CreateLogger<Program>();

            System.Timers.Timer timer = new(10000);
            int count = 0;
            timer.Elapsed += (source, e) =>
            {
                logger.LogInformation(count++,"Test info logs ---{DateTime}---", DateTime.UtcNow);
                logger.LogWarning(count++, "Test warning logs ---{DateTime}---", DateTime.UtcNow);
                logger.LogError(count++, "Test error logs ---{DateTime}---", DateTime.UtcNow);
            };
            timer.Enabled = true;
            timer.Start();

            await host.RunAsync();

        }
	}
}
