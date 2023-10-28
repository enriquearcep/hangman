using Hangman.Helpers;
using Hangman.Views;

namespace Hangman;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		if(SessionHelper.IsAuthenticate())
		{
            MainPage = new NavigationPage(new MainPage());
        }
		else
		{
            MainPage = new NavigationPage(new SignInView());
        }
	}
}