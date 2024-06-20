using Shared.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Shared.MsLogger
{
	[ProviderAlias("BlazorLogger")]
	internal sealed class BlazorLoggerProvider : ILoggerProvider
	{
		private readonly IDisposable? _onChangeToken;
		private readonly IDataStore _dataStore;
		private LoggerConfiguration _currentConfig;
		private readonly ConcurrentDictionary<string, BlazorMSLogger> _loggers = new();


		public BlazorLoggerProvider(IOptionsMonitor<LoggerConfiguration> config, IDataStore dataStore)
		{
			_currentConfig = config.CurrentValue;
			_onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
			_dataStore = dataStore;
		}

		public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new BlazorMSLogger(_dataStore, name, GetConfiguration));

		private LoggerConfiguration GetConfiguration() => _currentConfig;

		public void Dispose()
		{
			_loggers.Clear();

			_onChangeToken?.Dispose();
		}
	}
}
