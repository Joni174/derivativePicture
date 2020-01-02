using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using Outline.Annotations;
using Outline.ImageLogic;

namespace Outline.ViewModel
{
    public class ImageManager : INotifyPropertyChanged
    {
        private BitmapImage _openedImage;
        private WriteableBitmap _resultingImage;

        public BitmapImage OpenedImage{
            get => _openedImage;
            set
            {
                _openedImage = value;
                OnPropertyChanged(nameof(OpenedImage));
            }
        }

        public WriteableBitmap ResultingImage{
            get => _resultingImage;
            set
            {
                _resultingImage = value;
                OnPropertyChanged(nameof(ResultingImage));
            }
        }

        public ICommand OpenImage { get; private set; }

        public ImageManager()
        {
            OpenImage = new RelayCommand(o =>
            {
                OpenedImage = OpenImageFromFileSystem();
                var im = new ImageProcessor(OpenedImage);
                ResultingImage = im.ProcessWithFilter(new object());
            });
            OpenImage.CanExecute(true);
        }

        private BitmapImage OpenImageFromFileSystem()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
//                return File.ReadAllText(openFileDialog.FileName);
                return new BitmapImage(new Uri(openFileDialog.FileName));
            return new BitmapImage();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}