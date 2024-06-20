using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Shared.MsLogger.Extensions
{
	public static class BlazorMsLoggerExtensions
	{
		public static ILoggingBuilder AddBlazorLogger(this ILoggingBuilder builder)
		{
			builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, BlazorLoggerProvider>());

			return builder;
		}

		public static ILoggingBuilder AddBlazorLogger(this ILoggingBuilder builder, Action<LoggerConfiguration> configure)
		{
			builder.AddBlazorLogger();

			builder.Services.Configure(configure);

			return builder;
		}
	}
}
