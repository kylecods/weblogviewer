using Shared.Data;
using Shared.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace Shared.MsLogger
{
    internal sealed class BlazorMSLogger (IDataStore dataStore,string name, Func<LoggerConfiguration> configuration) : ILogger
	{
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

		public bool IsEnabled(LogLevel logLevel) => configuration().LogColors.ContainsKey(logLevel);

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			if(!IsEnabled(logLevel))
			{
				return;
			}

			var config = configuration();

			dataStore.AddEntry(new LogModel
			{
				EventId = eventId.Id,
				LogLevel = logLevel,
				Timestamp = DateTime.UtcNow,
                Color = config.LogColors[logLevel],
				State = state,
				Exception = exception?.Message ?? string.Empty,
            });
        }
	}
}
