using System.ComponentModel;

namespace Hangman;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    #region Properties
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

    private string attempsMessage;
    public string AttempsMessage
    {
        get => attempsMessage;

        set
        {
            attempsMessage = value;
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
        AttempsMessage = "Intentos restantes: 6";
        CurrentImage = "img0.jpg";

        BindingContext = this;

		LoadWords();

		SetRandomWord();

        CalculateWord(answer, guessed);
	}

    private void LoadWords()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("spanish.dic").Result;
        using var reader = new StreamReader(stream);

        while(reader.Peek() is not -1)
		{
			words.Add(reader.ReadLine().Split("/").FirstOrDefault().ToUpper());
		}
    }

	private void SetRandomWord()
	{
		answer = words[new Random().Next(0, words.Count)];
	}

	private void CalculateWord(string answer, List<char> guessed)
	{
		var temp = answer.Select(s => (guessed.Any(a => s.ToString().Equals(a.ToString(), StringComparison.OrdinalIgnoreCase)) ? s : '_')).ToArray();

        Spotlight = string.Join(' ', temp);
	}

    private void ShowAnswerButton_Clicked(object sender, EventArgs e)
    {
        CalculateWord(answer, answer.ToArray().ToList());
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
        if(!guessed.Any(a => a.ToString().Equals(letter.ToString(), StringComparison.OrdinalIgnoreCase)))
        {
            guessed.Add(letter);
        }

        if(answer.Any(a => a.ToString().Equals(letter.ToString(), StringComparison.OrdinalIgnoreCase)))
        {
            CalculateWord(answer, guessed);

            CheckIfGameWon();
        }
        else
        {
            mistakes++;

            UpdateStatus();

            CurrentImage = $"img{mistakes}.jpg";

            CheckIfGameOver();
        }
    }

    private void CheckIfGameWon()
    {
        if(Spotlight.Replace(" ", string.Empty).Equals(answer, StringComparison.OrdinalIgnoreCase))
        {
            StatusMessage = "¡Has ganado!";
            EnableButtons(false);
        }
    }

    private void UpdateStatus()
    {
        AttempsMessage = $"Intentos restantes: {6 - mistakes}";
    }

    private void CheckIfGameOver()
    {
        if(mistakes == 6)
        {
            StatusMessage = "¡Has perdido!";
            EnableButtons(false);
        }
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
        CurrentImage = "img0.jpg";
        StatusMessage = string.Empty;
        SetRandomWord();
        CalculateWord(answer, guessed);
        UpdateStatus();
        EnableButtons();
    }
}