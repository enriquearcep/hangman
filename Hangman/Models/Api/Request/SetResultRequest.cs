namespace Hangman.Models.Api.Request
{
    public class SetResultRequest
    {
        public string Word { get; set; }
        public int Mistakes { get; set; }
        public bool Won { get; set; }
    }
}