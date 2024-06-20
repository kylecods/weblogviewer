using Shared.Models;
using System.Collections.ObjectModel;

namespace Shared.Data
{
	public interface IDataStore
	{
		public ObservableCollection<LogModel> Entries { get; }

		public void AddEntry(LogModel model);
	}
}
