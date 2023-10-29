namespace Hangman.Helpers
{
    public static class SessionHelper
    {
        public static void Set(string accessToken)
        {
            Preferences.Set("Session", accessToken);
            Preferences.Set("ExpiresAt", DateTime.Now.AddDays(5).AddHours(-1));
        }

        public static bool IsAuthenticate()
        {
            try
            {
                var expiresAt = Preferences.Get("ExpiresAt", DateTime.Now.AddHours(-1));

                return expiresAt > DateTime.Now;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetToken()
        {
            try
            {
                if(!IsAuthenticate())
                {
                    return string.Empty;
                }

                return Preferences.Get("Session", string.Empty);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void CloseSession()
        {
            Preferences.Remove("Session");
            Preferences.Remove("ExpiresAt");
        }
    }
}