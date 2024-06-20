using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace WebLogViewer.Shared
{
	public class BaseComponent<TViewModel> : ComponentBase where TViewModel : BaseViewModel
	{
		[Inject]
		[NotNull]
		protected TViewModel ViewModel { get; set; }

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		protected override void OnInitialized()
		{
			ViewModel.PropertyChanged += (_, _) => StateHasChanged();
		}
	}

	public class BaseViewModel : ObservableObject
	{

	}
}
