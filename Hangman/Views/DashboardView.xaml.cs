using Hangman.ViewModels;

namespace Hangman.Views;

public partial class DashboardView : ContentPage
{
	public DashboardView()
	{
        InitializeComponent();
        BindingContext = new DashboardViewModel();
    }
}