using Hangman.Models.Api;
using Hangman.Models.Api.Request;
using Hangman.Models.Api.Response;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hangman.Helpers;

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

        public async Task<SignUpResponse> SignUp(SignUpRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(new HttpRequestMessage()
                    {
                        Content = requestContent,
                        RequestUri = new Uri($"{GetApiUrl()}/account/sign-up"),
                        Method = HttpMethod.Post
                    });

                    var responseText = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonSerializer.Deserialize<SignUpResponse>(responseText);
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

        public async Task<bool> SetResult(SetResultRequest request)
        {
			try
			{
                var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", SessionHelper.GetToken());

                    var response = await client.SendAsync(new HttpRequestMessage()
                    {
                        Content = requestContent,
                        RequestUri = new Uri($"{GetApiUrl()}/game/set-result"),
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

        public async Task<GetStatsResponse> GetStats()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", SessionHelper.GetToken());

                    var response = await client.SendAsync(new HttpRequestMessage()
                    {
                        RequestUri = new Uri($"{GetApiUrl()}/game/get-stats"),
                        Method = HttpMethod.Get
                    });

                    var jsonResponse = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonSerializer.Deserialize<GetStatsResponse>(jsonResponse);
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
    }
}