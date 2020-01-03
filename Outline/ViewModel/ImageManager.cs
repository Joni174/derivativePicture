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
using Outline.Annotations;


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
//                if (value.GetType() == typeof(Image))
//                {
//                    
//                    Console.WriteLine(value.Source);
//                    Console.WriteLine(value.Source);
//                }
                _resultingImage = value;
                OnPropertyChanged(nameof(ResultingImage));
            }
        }

        public ICommand OpenImage { get; private set; }

        public ImageManager()
        {
//            Stream imageStreamSource = new FileStream("fane.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
//            JpegBitmapDecoder decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
//            OpenedImage = decoder.Frames[0];

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"C:\Users\Jonas\Pictures\DoctorWhoWallpaper.jpg");
            image.DecodePixelWidth = 50;
            image.EndInit();
            OpenedImage = image;

            
            var pixels = GetPixel(OpenedImage);
            
            OpenImage = new RelayCommand(o =>
            {
                OpenedImage = CreateFromPixel(image.PixelWidth, image.PixelHeight, pixels);
                
                /*OpenedImage = OpenImageFromFileSystem();

                FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
                newFormatedBitmapSource.BeginInit();
                newFormatedBitmapSource.Source = OpenedImage;

                List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
                colors.Add(System.Windows.Media.Colors.Red);
                colors.Add(System.Windows.Media.Colors.Blue);
                BitmapPalette myPalette = new BitmapPalette(colors);

                // Set the DestinationPalette property to the custom palette created above.
                newFormatedBitmapSource.DestinationPalette = myPalette;

                // Set the DestinationFormat to the palletized pixel format of Indexed1.
                newFormatedBitmapSource.DestinationFormat = PixelFormats.Indexed1;
                newFormatedBitmapSource.EndInit();
                
                ResultingImage = newFormatedBitmapSource;*/
            });
            OpenImage.CanExecute(true);
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

        private BitmapSource CreateFromPixel(int width, int height, byte[] pixels)
        {
            Console.WriteLine(PixelFormats.Bgr32.BitsPerPixel);
            BitmapPalette myPalette = BitmapPalettes.Halftone256;
            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Bgr32,
                myPalette,
                pixels,
                width*4);
            return image;
        }

        private byte[] GetPixel(BitmapSource image)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int stride = (image.PixelWidth * image.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[stride * height];
            image.CopyPixels(pixels, stride, 0);

            int i = 0;
            var query = from s in pixels
                let num = i++
                group s by num / 4 into g
                select g.ToArray();

//            byte[][] groupedPixel = query.ToArray();
            return pixels;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}