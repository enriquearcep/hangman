using System.ComponentModel;
using System.Diagnostics;

namespace Hangman;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
	#region Global variables
	List<string> words = new List<string>();
	string answer = string.Empty;
	#endregion

	public MainPage()
	{
		InitializeComponent();

		LoadWords();

		SetRandomWord();
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
}