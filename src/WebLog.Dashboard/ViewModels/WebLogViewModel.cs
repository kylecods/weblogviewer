using Shared.Data;

namespace WebLog.Dashboard.ViewModels
{
	public sealed class WebLogViewModel(IDataStore dataStore) : BaseViewModel
	{
		public IDataStore DataStore { get; set; } = dataStore;
	}
}
