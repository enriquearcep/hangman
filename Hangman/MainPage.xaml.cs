using System.ComponentModel;
using System.Diagnostics;

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
    #endregion

    #region Global variables
    List<string> words = new List<string>();
	string answer = string.Empty;
    List<char> guessed = new List<char>();
    #endregion

    public MainPage()
	{
		InitializeComponent();

        KeyboardLetters.AddRange("ABCDEFGHIJKLMNÑOPQRSTUVWXYZ");

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

		Debug.WriteLine(answer);
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
    }

    private void CheckIfGameWon()
    {
        if(Spotlight.Replace(" ", string.Empty).Equals(answer, StringComparison.OrdinalIgnoreCase))
        {
            StatusMessage = "¡Has ganado!";
        }
    }
}