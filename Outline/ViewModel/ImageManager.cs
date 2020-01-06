using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Outline.Annotations;
using Outline.ImageLogic;


//TODO: Reordering of program structure

namespace Outline.ViewModel
{
    public class ImageManager : INotifyPropertyChanged
    {
        private BitmapSource _openedImage;
        private BitmapSource _resultingImage;

        public BitmapSource OpenedImage{
            get => _openedImage;
            set
            {
                _openedImage = value;
                OnPropertyChanged(nameof(OpenedImage));
            }
        }
        
        public BitmapSource ResultingImage{
            get => _resultingImage;
            set
            {
                _resultingImage = value;
                OnPropertyChanged(nameof(ResultingImage));
            }
        }
        public ICommand OpenImage { get; private set; }
        public ICommand ExportImage { get; private set; }
        public ImageManager()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"C:\Users\Jonas\Pictures\DoctorWhoWallpaper.jpg");
            image.DecodePixelWidth = 400;
            image.EndInit();
            OpenedImage = image;

            OpenImage = new RelayCommand(o =>
            {
                OpenedImage = OpenImageFromFileSystem();
                ImageProcessor imageProcessor = new ImageProcessor(OpenedImage);
                ResultingImage = imageProcessor.ProcessWithFilter();
            });
            
            ExportImage = new RelayCommand(o =>
            {
                WriteImageToFileSystem(ResultingImage);
            });
            OpenImage.CanExecute(true);
        }

        private void WriteImageToFileSystem(BitmapSource resultingImage)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resultingImage));
                using (FileStream file = File.OpenWrite(saveFileDialog.FileName))
                {
                    encoder.Save(file);
                }
            }
        }

        private BitmapImage OpenImageFromFileSystem()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            BitmapImage image = new BitmapImage();
            if(openFileDialog.ShowDialog() == true)
            {
                image = new BitmapImage(new Uri(openFileDialog.FileName));
            }
            return image;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}