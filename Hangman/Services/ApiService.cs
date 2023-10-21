using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hangman.Services
{
    public class ApiService
    {
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

                    if(response.IsSuccessStatusCode)
                    {
                        var jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return JsonSerializer.Deserialize<Stats>(jsonResponse);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
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