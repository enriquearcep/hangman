using Hangman.Helpers;
using Hangman.Models;
using Hangman.Models.Api.Request;
using Hangman.Services;
using Hangman.Views;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Hangman.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        private List<Life> lifes;
        public List<Life> Lifes
        {
            get => lifes;

            set
            {
                lifes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lifes)));
            }
        }

        private string spotlight;
        public string Spotlight
        {
            get => spotlight;

            set
            {
                spotlight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Spotlight)));
            }
        }

        private List<Models.Game.Keyboard> keyboardLetters = new List<Models.Game.Keyboard>();
        public List<Models.Game.Keyboard> KeyboardLetters
        {
            get => keyboardLetters;

            set
            {
                keyboardLetters = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KeyboardLetters)));
            }
        }

        private string statusMessage;
        public string StatusMessage
        {
            get => statusMessage;

            set
            {
                statusMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }

        private string _currentImage;
        public string CurrentImage
        {
            get => _currentImage;

            set
            {
                _currentImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentImage)));
            }
        }

        private bool _showAnswerIsVisible;
        public bool ShowAnswerIsVisible
        {
            get => _showAnswerIsVisible;

            set
            {
                _showAnswerIsVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowAnswerIsVisible)));
            }
        }

        private bool resultGameIsVisible;
        public bool ResultGameIsVisible
        {
            get => resultGameIsVisible;

            set
            {
                resultGameIsVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultGameIsVisible)));
            }
        }

        private bool recentlyWinned;
        public bool RecentlyWinned
        {
            get => recentlyWinned;

            set
            {
                recentlyWinned = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecentlyWinned)));
            }
        }

        private int totalWon;
        public int TotalWon
        {
            get => totalWon;

            set
            {
                totalWon = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalWon)));
            }
        }

        private int totalLost;
        public int TotalLost
        {
            get => totalLost;

            set
            {
                totalLost = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalLost)));
            }
        }

        private int winStreak;
        public int WinStreak
        {
            get => winStreak;

            set
            {
                winStreak = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinStreak)));
            }
        }

        private int longestWinStreak;
        public int LongestWinStreak
        {
            get => longestWinStreak;

            set
            {
                longestWinStreak = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LongestWinStreak)));
            }
        }
        #endregion

        #region Global variables
        List<string> words = new List<string>();
        string answer = string.Empty;
        int mistakes = 0;
        List<char> guessed = new List<char>();
        #endregion

        #region Commands
        public ICommand NavigateToMenuCommand => new Command(async () => await NavigateToMenu());
        public ICommand NewGameCommand => new Command(NewGame);
        public ICommand ShowAnswerCommand => new Command(ShowAnswer);
        #endregion

        #region Constructors
        public DashboardViewModel()
        {
            // KeyboardLetters.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            LoadKeyboard();
            CurrentImage = "img_0.jpg";
            ShowAnswerIsVisible = true;
            ResultGameIsVisible = false;
            RecentlyWinned = false;

            RestartLifes();
            LoadWords();
            SetRandomWord();
            CalculateWord(answer, guessed);
            GetStats();
        }
        #endregion

        #region Functions
        private void LoadKeyboard()
        {
            KeyboardLetters = new List<Models.Game.Keyboard>();

            var listKeys = new List<char>();

            listKeys.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

            foreach (char key in listKeys)
            {
                var model = new Models.Game.Keyboard() { Key = key, IsEnabled = true };
                model.KeyPressed += KeyPressed;

                KeyboardLetters.Add(model);
            }
        }

        private void RestartLifes()
        {
            Lifes = new List<Life>()
            {
                new Life() { Alive = true },
                new Life() { Alive = true },
                new Life() { Alive = true },
                new Life() { Alive = true },
                new Life() { Alive = true },
                new Life() { Alive = true }
            };
        }

        private void LoadWords()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("spanish.dic").Result;
            using var reader = new StreamReader(stream);

            while (reader.Peek() is not -1)
            {
                var word = reader.ReadLine().ToUpper();

                if (word.Length > 3 && word.Length <= 8 && !word.Contains("Ñ"))
                {
                    words.Add(word);
                }
            }
        }

        private void SetRandomWord()
        {
            answer = words[new Random().Next(0, words.Count)];
        }

        private void CalculateWord(string answer, List<char> guessed)
        {
            var comparer = StringComparer.Create(new CultureInfo("es-ES", false), CompareOptions.IgnoreNonSpace);

            var temp = answer.Select(s => (guessed.Any(a => comparer.Equals(s.ToString(), a.ToString())) ? s : '_')).ToArray();

            Spotlight = string.Join(' ', temp);
        }

        private async void GetStats()
        {
            try
            {
                var api = new ApiService();

                var stats = await api.GetStats();

                if (stats is not null)
                {
                    TotalWon = stats.TotalWon;
                    TotalLost = stats.TotalLost;
                    WinStreak = stats.WinStreak;
                    LongestWinStreak = stats.LongestWinStreak;
                }
            }
            catch (Exception)
            {
                // Nothing
            }
        }

        private void NewGame()
        {
            mistakes = 0;
            guessed = new List<char>();
            CurrentImage = "img_0.jpg";
            StatusMessage = string.Empty;
            ShowAnswerIsVisible = true;
            ResultGameIsVisible = false;
            RecentlyWinned = false;
            RestartLifes();
            SetRandomWord();
            CalculateWord(answer, guessed);
            EnableButtons();
        }

        private void EnableButtons(bool enable = true)
        {
            foreach (var control in KeyboardLetters)
            {
                control.IsEnabled = enable;
            }
        }

        private void ShowAnswer()
        {
            guessed = answer.ToArray().ToList();

            CalculateWord(answer, guessed);

            ShowAnswerIsVisible = false;

            EnableButtons(false);
        }

        private void KeyPressed(char key)
        {
            HandleGuess(key);
        }

        private void HandleGuess(char letter)
        {
            var comparer = StringComparer.Create(new CultureInfo("es-ES", false), CompareOptions.IgnoreNonSpace);

            if (!guessed.Any(a => comparer.Equals(a.ToString(), letter.ToString())))
            {
                guessed.Add(letter);
            }

            if (answer.Any(a => comparer.Equals(a.ToString(), letter.ToString())))
            {
                CalculateWord(answer, guessed);

                SoundsHelper.Play(SoundType.HIT);

                CheckIfGameWon();
            }
            else
            {
                Vibration.Default.Vibrate(TimeSpan.FromSeconds(1));

                mistakes++;

                RemoveALife();
                SoundsHelper.Play(SoundType.WRONG);

                CurrentImage = $"img_{mistakes}.jpg";

                CheckIfGameOver();
            }
        }

        private void RemoveALife()
        {
            if (Lifes is null)
            {
                return;
            }

            int index = Lifes.IndexOf(Lifes.FirstOrDefault(f => !f.Alive));

            Lifes[index == -1 ? Lifes.Count - 1 : index - 1].Alive = false;
        }

        private async void CheckIfGameWon()
        {
            var comparer = StringComparer.Create(new CultureInfo("es-ES", false), CompareOptions.IgnoreNonSpace);

            if (comparer.Equals(Spotlight.Replace(" ", string.Empty), answer))
            {
                StatusMessage = "¡HAS GANADO!";
                SetResult(true);
                SoundsHelper.Play(SoundType.WON);
                ShowAnswerIsVisible = false;
                ResultGameIsVisible = true;
                EnableButtons(false);
            }
        }

        private void CheckIfGameOver()
        {
            if (mistakes == 6)
            {
                StatusMessage = "¡HAS PERDIDO!";
                SetResult(false);
                SoundsHelper.Play(SoundType.GAME_OVER);
                ResultGameIsVisible = true;
                EnableButtons(false);
            }
        }

        private async void SetResult(bool won)
        {
            try
            {
                var api = new ApiService();

                var saved = await api.SetResult(new SetResultRequest()
                {
                    Word = answer,
                    Mistakes = mistakes,
                    Won = won
                });

                GetStats();
            }
            catch (Exception)
            {
                // Nothing
            }
        }

        private async Task NavigateToMenu()
        {
            await App.Current.MainPage.Navigation.PushAsync(new MenuView());
        }
        #endregion
    }
}