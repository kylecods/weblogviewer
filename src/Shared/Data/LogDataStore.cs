using Shared.Models;
using System.Collections.ObjectModel;

namespace Shared.Data
{
	public sealed class LogDataStore : IDataStore
	{
		private static readonly SemaphoreSlim _semaphore = new (1);

		public ObservableCollection<LogModel> Entries { get; } = [];

		public void AddEntry(LogModel model)
		{
			_semaphore.Wait();

			Entries.Add(model);

			_semaphore.Release();
		}
	}
}
