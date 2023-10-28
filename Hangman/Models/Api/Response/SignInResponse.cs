using System.Text.Json.Serialization;

namespace Hangman.Models.Api.Response
{
    public class SignInResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }
}