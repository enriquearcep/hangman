using Hangman.ViewModels;

namespace Hangman.Views;

public partial class MenuView : ContentPage
{
	public MenuView()
	{
		InitializeComponent();

		BindingContext = new MenuViewModel();
	}
}