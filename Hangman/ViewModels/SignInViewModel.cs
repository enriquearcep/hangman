using Hangman.Helpers;
using Hangman.Models.Api.Request;
using Hangman.Services;
using System.Windows.Input;

namespace Hangman.ViewModels
{
    public class SignInViewModel
    {
        #region Properties
        public string Email { get; set; }
        public string Password { get; set; }
        #endregion

        #region Commands
        public ICommand SignInCommand => new Command(SignIn);
        #endregion

        #region Constructors
        public SignInViewModel()
        {
            Email = "medss@enriquepz.com";
            Password = "$Kikin123";
        } 
        #endregion

        #region Functions
        private async void SignIn()
        {
            try
            {
                var api = new ApiService();

                var signed = await api.SignIn(new SignInRequest()
                {
                    Email = Email,
                    Password = Password
                });

                if (signed is null)
                {
                    var error = api.GetLastError();

                    await App.Current.MainPage.DisplayAlert(error.Title, error.Description, "OK");

                    return;
                }

                SessionHelper.Set(signed.AccessToken);

                App.Current.MainPage = new MainPage();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion
    }
}