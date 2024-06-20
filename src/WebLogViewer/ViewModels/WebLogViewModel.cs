using Shared.Data;
using WebLogViewer.Shared;

namespace WebLogViewer.ViewModels
{
	public sealed class WebLogViewModel(IDataStore dataStore) : BaseViewModel
	{
		public IDataStore DataStore { get; set; } = dataStore;
	}
}
