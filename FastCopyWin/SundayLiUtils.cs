using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FastCopyWin
{
    public struct PixelColor
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;
    }

    public class SundayLiUtils
    {
        /**
         * 对话框式保存图片
         */
        public static void SaveImage(BitmapSource bitmapSource) 
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Image Files (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg | All Files | *.*";
            sfd.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
            if (sfd.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                using FileStream stream = new(sfd.FileName, FileMode.Create);
                encoder.Save(stream);
            }
        }

        public static PixelColor[,] GetPixels2(BitmapSource source)
        {
            PixelColor[,] result = new PixelColor[source.PixelWidth, source.PixelHeight];

            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];
            source.CopyPixels(pixels, stride, 0);
            for (int y = 0; y < source.PixelHeight; y++)
            {
                for (int x = 0; x < source.PixelWidth; x++)
                {
                    int index = y * stride + 4 * x;
                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];
                    result[x, y] = new PixelColor()
                    {
                        Alpha = alpha,
                        Red = red,
                        Green = green,
                        Blue = blue
                    };
                }
            }
            return result;
        }

        public unsafe static BitmapSource CreateByOpacityColor(BitmapSource srcImage_)
        {
            BitmapSource srcImage = null;

            if (srcImage_.Format == System.Windows.Media.PixelFormats.Bgr32 || srcImage_.Format == System.Windows.Media.PixelFormats.Bgra32)
            {
                srcImage = srcImage_;
            }
            else
            {
                // 先进行像素格式转换，再拷贝像素数据     
                srcImage = new FormatConvertedBitmap(srcImage_, System.Windows.Media.PixelFormats.Bgr32, null, 0);
            }
            WriteableBitmap bmp_ = new WriteableBitmap(srcImage);
            bmp_.Lock();
            PixelColor[,] arr = GetPixels2(srcImage_);
            for (int i = 0; i < srcImage.PixelWidth; i++) 
            {
                for (int j = 0; j < srcImage.PixelHeight; j++) 
                {
                    byte[] ColorData = { arr[i, j].Blue, arr[i, j].Green, arr[i, j].Red, 255 }; // B G R
                    Int32Rect rect = new Int32Rect(i, j, 1, 1);
                    bmp_.WritePixels(rect, ColorData, 4, 0);
                }
            }
            bmp_.Unlock();
            return bmp_;
        }
    }
}
