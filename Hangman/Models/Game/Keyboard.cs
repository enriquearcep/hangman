using System.ComponentModel;
using System.Windows.Input;

namespace Hangman.Models.Game
{
    public class Keyboard : INotifyPropertyChanged
    {
        internal delegate void KeyPressedDelegate(char key);
        internal event KeyPressedDelegate KeyPressed;
        public event PropertyChangedEventHandler PropertyChanged;

        private char key;
        public char Key
        {
            get => key;
            
            set
            {
                if(key != value)
                {
                    key = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
                }
            }
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;

            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public ICommand PressedCommand => new Command(() => 
        {
            IsEnabled = false;
            KeyPressed(Key);
        });
    }
}