using System.ComponentModel;

namespace Hangman.Models
{
    public class Life : INotifyPropertyChanged
    {
        private bool alive;
        public bool Alive
        {
            get => alive;
            
            set
            {
                if(alive != value)
                {
                    alive = value;
                    ImageSource = value ? "\uE809" : "\uE808";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Alive)));
                }
            }
        }

        private string imageSource;
        public string ImageSource
        {
            get => imageSource;

            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
