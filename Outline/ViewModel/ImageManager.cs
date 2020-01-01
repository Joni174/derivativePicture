using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using Outline.Annotations;

namespace Outline.ViewModel
{
    public class ImageManager : INotifyPropertyChanged
    {
        private string _text;

        public String Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public ICommand OpenImage { get; private set; }


        public ImageManager()
        {
            OpenImage = new RelayCommand(o => { Text = GetFileContent();});
            OpenImage.CanExecute(true);
        }

        public string GetFileContent()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
                return File.ReadAllText(openFileDialog.FileName);
            return "Error Reading File";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}