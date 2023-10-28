using System.Text.Json.Serialization;

namespace Hangman.Models.Api.Response
{
    public class GetStatsResponse
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