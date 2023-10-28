using Hangman.ViewModels;

namespace Hangman.Views;

public partial class SignInView : ContentPage
{
	public SignInView()
	{
		InitializeComponent();

		BindingContext = new SignInViewModel();
	}
}