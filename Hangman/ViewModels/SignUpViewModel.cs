using Hangman.Helpers;
using Hangman.Models.Api.Request;
using Hangman.Services;
using System.Windows.Input;

namespace Hangman.ViewModels
{
    public class SignUpViewModel
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        #endregion

        #region Commands
        public ICommand SignUpCommand => new Command(SignUp);
        #endregion

        #region Functions
        private async void SignUp()
        {
            try
            {
                if(AreValidFields())
                {
                    var api = new ApiService();

                    var registered = await api.SignUp(new SignUpRequest()
                    {
                        Email = Email,
                        Password = Password,
                        FirstName = FirstName,
                        LastName = LastName
                    });

                    if (registered is null)
                    {
                        var error = api.GetLastError();

                        await App.Current.MainPage.DisplayAlert(error.Title, error.Description, "Aceptar");

                        return;
                    }

                    SessionHelper.Set(registered.AccessToken);

                    App.Current.MainPage = new MainPage();
                }
            }
            catch (Exception)
            {
                // Nothing
            }
        }

        private bool AreValidFields()
        {
            if(string.IsNullOrEmpty(FirstName))
            {
                App.Current.MainPage.DisplayAlert("Campo requerido", "Debes ingresar tu nombre.", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                App.Current.MainPage.DisplayAlert("Campo requerido", "Debes ingresar tu apellido.", "Aceptar");
                return false;
            }

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
