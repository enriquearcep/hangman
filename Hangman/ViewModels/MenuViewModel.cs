using Hangman.Helpers;
using Hangman.Views;
using System.Windows.Input;

namespace Hangman.ViewModels
{
    public class MenuViewModel
    {
        #region Commands
        public ICommand CloseSessionCommand => new Command(CloseSession);
        #endregion

        #region Functions
        private void CloseSession()
        {
            SessionHelper.CloseSession();

            App.Current.MainPage = new NavigationPage(new SignInView());
        }
        #endregion
    }
}