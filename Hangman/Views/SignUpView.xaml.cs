using Hangman.ViewModels;

namespace Hangman.Views;

public partial class SignUpView : ContentPage
{
	public SignUpView()
	{
		InitializeComponent();

		BindingContext = new SignUpViewModel();
	}
}