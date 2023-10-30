using Hangman.Helpers;
using Hangman.Models.Api.Request;
using Hangman.Services;
using Hangman.Views;
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
        public ICommand SignInCommand => new Command(async () => await SignIn());
        public ICommand RedirectToSignUpCommand => new Command(RedirectToSignUp);
        #endregion

        #region Constructors
        public SignInViewModel()
        {
            //Email = "medss@enriquepz.com";
            //Password = "$Kikin123";
        } 
        #endregion

        #region Functions
        private async Task SignIn()
        {
            try
            {
                if(AreValidFields())
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

                        await App.Current.MainPage.DisplayAlert(error.Title, error.Description, "Aceptar");

                        return;
                    }

                    SessionHelper.Set(signed.AccessToken);

                    App.Current.MainPage = new NavigationPage(new DashboardView());
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private void RedirectToSignUp()
        {
            App.Current.MainPage.Navigation.PushAsync(new SignUpView());
        }

        private bool AreValidFields()
        {
            if (string.IsNullOrEmpty(Email))
            {
                App.Current.MainPage.DisplayAlert("Campo requerido", "Debes ingresar tu correo electrónico.", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(Password))
            {
                App.Current.MainPage.DisplayAlert("Campo requerido", "Debes ingresar tu contraseña.", "Aceptar");
                return false;
            }

            return true;
        }
        #endregion
    }
}