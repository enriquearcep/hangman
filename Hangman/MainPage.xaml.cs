using Hangman.Models;
using Hangman.Models.Api.Request;
using Hangman.Services;
using Plugin.Maui.Audio;
using System.ComponentModel;
using System.Globalization;

namespace Hangman;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    #region Properties
    private List<Life> lifes;
    public List<Life> Lifes
    {
        get => lifes;

        set
        {
            lifes = value;
            OnPropertyChanged();
        }
    }

    private string spotlight;
    public string Spotlight
    {
        get => spotlight;
        
        set
        {
            spotlight = value;
            OnPropertyChanged();
        }
    }

    private List<char> keyboardLetters = new List<char>();
    public List<char> KeyboardLetters
    {
        get => keyboardLetters;

        set
        {
            keyboardLetters = value;
            OnPropertyChanged();
        }
    }

    private string statusMessage;
    public string StatusMessage
    {
        get => statusMessage;

        set
        {
            statusMessage = value;
            OnPropertyChanged();
        }
    }

    private string _currentImage;
    public string CurrentImage
    {
        get => _currentImage;

        set
        {
            _currentImage = value;
            OnPropertyChanged();
        }
    }

    private bool _showAnswerIsVisible;
    public bool ShowAnswerIsVisible
    {
        get => _showAnswerIsVisible;

        set
        {
            _showAnswerIsVisible = value;
            OnPropertyChanged();
        }
    }

    private bool resultGameIsVisible;
    public bool ResultGameIsVisible
    {
        get => resultGameIsVisible;

        set
        {
            resultGameIsVisible = value;
            OnPropertyChanged();
        }
    }

    private bool recentlyWinned;
    public bool RecentlyWinned
    {
        get => recentlyWinned;

        set
        {
            recentlyWinned = value;
            OnPropertyChanged();
        }
    }

    private int totalWon;
    public int TotalWon
    {
        get => totalWon;

        set
        {
            totalWon = value;
            OnPropertyChanged();
        }
    }

    private int totalLost;
    public int TotalLost
    {
        get => totalLost;

        set
        {
            totalLost = value;
            OnPropertyChanged();
        }
    }

    private int winStreak;
    public int WinStreak
    {
        get => winStreak;

        set
        {
            winStreak = value;
            OnPropertyChanged();
        }
    }

    private int longestWinStreak;
    public int LongestWinStreak
    {
        get => longestWinStreak;

        set
        {
            longestWinStreak = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Global variables
    List<string> words = new List<string>();
	string answer = string.Empty;
	int mistakes = 0;
    List<char> guessed = new List<char>();
    #endregion

    public MainPage()
	{
		InitializeComponent();

        KeyboardLetters.AddRange("ABCDEFGHIJKLMNÑOPQRSTUVWXYZ");
        CurrentImage = "img_0.jpg";
        ShowAnswerIsVisible = true;
        ResultGameIsVisible = false;
        RestartLifes();
        RecentlyWinned = false;

        BindingContext = this;

		LoadWords();

		SetRandomWord();

        CalculateWord(answer, guessed);

        GetStats();
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

    private void RemoveALife()
    {
        if (Lifes is null)
        {
            return;
        }

        int index = Lifes.IndexOf(Lifes.FirstOrDefault(f => !f.Alive));

        Lifes[index == -1 ? Lifes.Count - 1 : index - 1].Alive = false;
    }

    private void LoadWords()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("spanish.dic").Result;
        using var reader = new StreamReader(stream);

        while(reader.Peek() is not -1)
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

    private void ShowAnswerButton_Clicked(object sender, EventArgs e)
    {
        guessed = answer.ToArray().ToList();

        CalculateWord(answer, guessed);

        ShowAnswerIsVisible = false;

        EnableButtons(false);
    }

    private void KeyboardButton_Clicked(object sender, EventArgs e)
    {
        var keyboardButton = sender as Button;

        if (keyboardButton != null)
        {
            keyboardButton.IsEnabled = false;

            HandleGuess(keyboardButton.Text.FirstOrDefault());
        }
    }

    private void HandleGuess(char letter)
    {
        var comparer = StringComparer.Create(new CultureInfo("es-ES", false), CompareOptions.IgnoreNonSpace);

        if (!guessed.Any(a => comparer.Equals(a.ToString(), letter.ToString())))
        {
            guessed.Add(letter);
        }

        if(answer.Any(a => comparer.Equals(a.ToString(), letter.ToString())))
        {
            CalculateWord(answer, guessed);

            PlayHitSound();

            CheckIfGameWon();
        }
        else
        {
            Vibration.Default.Vibrate(TimeSpan.FromSeconds(1));

            mistakes++;

            RemoveALife();
            PlayWrongSound();

            CurrentImage = $"img_{mistakes}.jpg";

            CheckIfGameOver();
        }
    }

    private async void CheckIfGameWon()
    {
        var comparer = StringComparer.Create(new CultureInfo("es-ES", false), CompareOptions.IgnoreNonSpace);

        if (comparer.Equals(Spotlight.Replace(" ", string.Empty), answer))
        {
            StatusMessage = "¡HAS GANADO!";
            SetResult(true);
            PlayWonSound();
            ShowAnswerIsVisible = false;
            ResultGameIsVisible = true;
            EnableButtons(false);
        }
    }

    private async void GetStats()
    {
        try
        {
            var api = new ApiService();

            var stats = await api.GetStats();

            if(stats is not null)
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

    private string GetDeviceId()
    {
        return $"{DeviceInfo.Model}-{DeviceInfo.Manufacturer}-{DeviceInfo.Name}".Normalize();
    }

    private void CheckIfGameOver()
    {
        if(mistakes == 6)
        {
            StatusMessage = "¡HAS PERDIDO!";
            SetResult(false);
            PlayGameOverSound();
            ResultGameIsVisible = true;
            EnableButtons(false);
        }
    }

    private void PlayHitSound()
    {
        var audioPlayer = AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync("hit.wav").Result);

        audioPlayer.Play();
    }

    private void PlayWrongSound()
    {
        var audioPlayer = AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync("wrong.wav").Result);

        audioPlayer.Play();
    }

    private async Task PlayWonSound()
    {
        var audioPlayer = AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync("won.wav").Result);

        audioPlayer.Play();

        RecentlyWinned = true;

        await Task.Delay(4000);

        RecentlyWinned = false;
    }

    private void PlayGameOverSound()
    {
        var audioPlayer = AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync("game_over.wav").Result);

        audioPlayer.Play();
    }

    private void EnableButtons(bool enable = true)
    {
        foreach (var control in keyboardContainer.Children)
        {
            var keyboardButton = control as Button;

            if (keyboardButton != null)
            {
                keyboardButton.IsEnabled = enable;
            }
        }
    }

    private void StartNewGameButton_Clicked(object sender, EventArgs e)
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
}