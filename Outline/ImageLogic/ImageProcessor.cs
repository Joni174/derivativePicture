using System;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Outline.ImageLogic
{
    class ImageProcessor
    {
        public BitmapSource Image { get; set; }

        public ImageProcessor(BitmapSource image)
        {
            Image = image;
        }

        public BitmapSource ProcessWithFilter()
        {
            int width = Image.PixelWidth;
            var pixels = GetPixel(Image);
            int i = 0;
            var query = from s in pixels
                let num = i++
                group s by num / 4 into g
                select g.ToArray();
            byte[][] groupedPixel = query.ToArray();
            i = 0;
            var other = from s in groupedPixel
                let num = i++
                group s by num / width into g
                select g.ToArray();
            byte[][][] pixelField = other.ToArray();
            byte[][][] otherField = new byte[pixelField.Length-2][][];

            for (i = 1; i < pixelField.Length-1; i++)
            {
                otherField[i-1] = new byte[pixelField[i-1].Length-2][];
                for (int j = 1; j < pixelField[i].Length-1; j++)
                {
                    byte[][] compare = new[]
                    {
                        pixelField[i - 1][j - 1],
                        pixelField[i - 1][j + 1],
                        pixelField[i + 1][j - 1],
                        pixelField[i + 1][j + 1],
                    };

                    int biggestContrast = 0;
                    for (int k = 0; k < compare.Length; k++)
                    {
                        int kr = pixelField[i][j][0] - compare[k][0];
                        int kb = pixelField[i][j][1] - compare[k][1];
                        int kg = pixelField[i][j][2] - compare[k][2];
                        int contrast = (Math.Abs(kr) + Math.Abs(kb) + Math.Abs(kg)) / 3;
                        if (contrast > biggestContrast) biggestContrast = contrast;
                    }

                    int greyTone = 255 - biggestContrast;
//                    otherField[i - 1][j - 1] = new[] {(byte)1, (byte)1, (byte)1, (byte)255};
                    otherField[i - 1][j - 1] = new[] {(byte)greyTone, (byte)greyTone, (byte)greyTone, (byte)255};
                }
            }

            byte[] solution = new byte[otherField.Length * otherField[0].Length * 4];
            int c = 0;
            for (int j = 0; j < otherField.Length; j++)
            {
                for (int k = 0; k < otherField[j].Length; k++)
                {
                    
                    for (int l = 0; l < otherField[j][k].Length; l++)
                    {
                        solution[c] = otherField[j][k][l];
                        c++;
                    }
                }
            }

            Console.WriteLine($"rows: {pixelField.Length}, columns: {pixelField[0].Length}");
            return CreateFromPixel(Image.PixelWidth - 2, Image.PixelHeight - 2, solution);
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
    }
}