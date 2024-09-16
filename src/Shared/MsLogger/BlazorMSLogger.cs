using System.Net.Http.Json;
using Shared.Data;
using Shared.Models;
using Microsoft.Extensions.Logging;
using Shared.FileLogs;


namespace Shared.MsLogger
{
    internal sealed class BlazorMSLogger (IDataStore dataStore,string name, Func<LoggerConfiguration> configuration) : ILogger
    {
	    private HttpClient client = new ();
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
				Timestamp = DateTime.Now,
                Color = config.LogColors[logLevel],
				State = state,
				Exception = exception?.Message ?? DateTime.Now.ToString(),
            });
        }
	}
}
