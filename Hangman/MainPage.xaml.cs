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
		var temp = answer.Select(s => (guessed.IndexOf(s) >= 0 ? s : '_')).ToArray();

        Spotlight = string.Join(' ', temp);
	}
}