using Hangman.Models.Api;
using Hangman.Models.Api.Request;
using Hangman.Models.Api.Response;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hangman.Services
{
    public class ApiService
    {
        private readonly ApiError _error;

        public ApiService()
        {
            _error = new ApiError();
        }

        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(new HttpRequestMessage()
                    {
                        Content = requestContent,
                        RequestUri = new Uri($"{GetApiUrl()}/account/sign-in"),
                        Method = HttpMethod.Post
                    });

                    var responseText = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonSerializer.Deserialize<SignInResponse>(responseText);
                    }
                    else
                    {
                        LoadError($"Error - {response.StatusCode}", responseText);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LoadError(ex);
                return null;
            }
        }

        public async Task<bool> SetResult(string deviceId, string word, int mistakes, bool won)
        {
			try
			{
                using(var client = new HttpClient())
                {
                    var response = await client.SendAsync(new HttpRequestMessage()
                    {
                        RequestUri = new Uri($"{GetApiUrl()}/hangman/set-result?deviceId={deviceId}&word={word}&mistakes={mistakes}&won={won}"),
                        Method = HttpMethod.Post
                    });

                    return response.IsSuccessStatusCode;
                }
			}
			catch (Exception ex)
			{
                return false;
			}
        }

        public async Task<Stats> GetStats(string deviceId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(new HttpRequestMessage()
                    {
                        RequestUri = new Uri($"{GetApiUrl()}/hangman/get-stats?deviceId={deviceId}"),
                        Method = HttpMethod.Get
                    });

                    var jsonResponse = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonSerializer.Deserialize<Stats>(jsonResponse);
                    }
                    else
                    {
                        LoadError($"Error - {response.StatusCode}", jsonResponse);

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LoadError(ex);

                return null;
            }
        }

        private void LoadError(string title, string description)
        {
            _error.Title = title;
            _error.Description = description;
        }

        private void LoadError(Exception ex)
        {
            _error.Title = ex.Message;
            _error.Description = ex.StackTrace;
        }

        public ApiError GetLastError()
        {
            return _error;
        }

        private string GetApiUrl()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("config.json").Result;
            using var reader = new StreamReader(stream);

            var json = reader.ReadToEnd();

            var config = JsonSerializer.Deserialize<ConfigFile>(json);

            return config.ApiUrl;
        }

        public class ConfigFile
        {
            public string ApiUrl { get; set; }
        }

        public class Stats
        {
            [JsonPropertyName("totalWon")]
            public int TotalWon { get; set; }

            [JsonPropertyName("totalLost")]
            public int TotalLost { get; set; }

            [JsonPropertyName("winStreak")]
            public int WinStreak { get; set; }

            [JsonPropertyName("longestWinStreak")]
            public int LongestWinStreak { get; set; }
        }
    }
}