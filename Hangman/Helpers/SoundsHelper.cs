using Plugin.Maui.Audio;

namespace Hangman.Helpers
{
    public enum SoundType
    {
        HIT,
        WRONG,
        WON,
        GAME_OVER
    }

    public static class SoundsHelper
    {
        public static void Play(SoundType type)
        {
            string fileName = type switch {
                SoundType.HIT => "hit.wav",
                SoundType.WRONG => "wrong.wav",
                SoundType.WON => "won.wav",
                SoundType.GAME_OVER => "game_over.wav",
                _ => string.Empty
            };

            Play(fileName);
        }

        private static void Play(string fileName)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var audioPlayer = AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync(fileName).Result);

            audioPlayer.Play();
        }
    }
}
