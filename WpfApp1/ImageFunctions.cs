using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using Rectangle = System.Drawing.Rectangle;
using System;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace STFC_EventLogger
{
    public static class ImageFunctions
    {
        /// <summary>
        /// Crops the and resize image.
        /// </summary>
        /// <param name="img">The image</param>
        /// <param name="targetWidth">Width of the target.</param>
        /// <param name="targetHeight">Height of the target.</param>
        /// <param name="x1">The position x1.</param>
        /// <param name="y1">The position y1.</param>
        /// <param name="x2">The position x2.</param>
        /// <param name="y2">The position y2.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>Image of the cropped and resized image.</returns>
        public static Image CropAndResizeImage(Image img, int targetWidth, int targetHeight, int x1, int y1, int x2, int y2, ImageFormat imageFormat)
        {
            using var memStream = new MemoryStream();
            using (var bmp = new Bitmap(targetWidth, targetHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    int width = x2 - x1;
                    int height = y2 - y1;

                    g.DrawImage(img, new Rectangle(0, 0, targetWidth, targetHeight), x1, y1, width, height, GraphicsUnit.Pixel);
                }
                bmp.Save(memStream, imageFormat);
            }
            return Image.FromStream(memStream);
        }

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="img">The image</param>
        /// <param name="targetWidth">Width of the target.</param>
        /// <param name="targetHeight">Height of the target.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>Image of the resized image.</returns>
        public static Image ResizeImage(Image img, int targetWidth, int targetHeight, ImageFormat imageFormat)
        {
            return CropAndResizeImage(img, targetWidth, targetHeight, 0, 0, img.Width, img.Height, imageFormat);
        }

        /// <summary>
        /// Crops the image.
        /// </summary>
        /// <param name="img">The image</param>
        /// <param name="x1">The position x1.</param>
        /// <param name="y1">The position y1.</param>
        /// <param name="x2">The position x2.</param>
        /// <param name="y2">The position y2.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>Image of the cropped image.</returns>
        public static Image CropImage(Image img, int x1, int y1, int x2, int y2, ImageFormat imageFormat)
        {
            return CropAndResizeImage(img, x2 - x1, y2 - y1, x1, y1, x2, y2, imageFormat);
        }
        public static string CropImage(string imgFile, int x1, int y1, int x2, int y2, ImageFormat imageFormat)
        {
            string tmp = Path.GetTempFileName();

            Task.Run(() =>
            {
                Image img = Image.FromFile(imgFile);
                Image cropped = CropAndResizeImage(img, x2 - x1, y2 - y1, x1, y1, x2, y2, imageFormat);

                cropped.Save(tmp);

                img.Dispose();
                cropped.Dispose();
            });

            return tmp;
        }

        public static byte[] ToByteArray(this System.Drawing.Image image)
        {
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public static BitmapImage? ImageFromBuffer(Image? img)
        {
            if (img == null)
                return null;

            byte[] bytes = img.ToByteArray();
            MemoryStream stream = new(bytes);
            BitmapImage image = new();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static BitmapImage BitmapImageFromFile(string uri)
        {
            // Create source
            BitmapImage myBitmapImage = new();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(uri);
            myBitmapImage.EndInit();

            return myBitmapImage;
        }


        public static string CropImage(string bmpSrc, int x1, int y1, int x2, int y2)
        {
            using Bitmap bmp = new(bmpSrc);
            Rectangle srcRect = Rectangle.FromLTRB(x1, y1, x2, y2);
            using Bitmap dest = new(srcRect.Width, srcRect.Height);
            Rectangle destRect = new(0, 0, srcRect.Width, srcRect.Height);
            using (Graphics graphics = Graphics.FromImage(dest))
            {
                graphics.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
            }
            string destFile = Path.GetTempFileName();
            dest.Save(destFile);
            return destFile;
        }



        internal static Image InvertUnsafe(Image imgSource)
        {

            Bitmap bmpDest = new(imgSource);

            BitmapData bmpSource = bmpDest.LockBits(new Rectangle(0, 0,
               bmpDest.Width, bmpDest.Height), ImageLockMode.ReadWrite,
               PixelFormat.Format32bppArgb);

            int intStride = bmpSource.Stride;
            IntPtr iptrScan0 = bmpSource.Scan0;

            unsafe   // Project, Properties, Build, Allow Unsafe Code
            {

                byte* p = (byte*)(void*)iptrScan0;

                int intOffset = intStride - bmpDest.Width * 4;
                int intWidth = bmpDest.Width;

                for (int y = 0; y < bmpDest.Height; y++)
                {
                    for (int x = 0; x < intWidth; x++)
                    {
                        p[0] = (byte)(255 - p[0]);
                        p[1] = (byte)(255 - p[1]);
                        p[2] = (byte)(255 - p[2]);

                        p += 4;
                    }
                    p += intOffset;
                }
            }

            bmpDest.UnlockBits(bmpSource);

            return bmpDest;
        }
        internal static Image InvertColorMatrix(Image imgSource)
        {
            Bitmap bmpDest = new(imgSource.Width, imgSource.Height);

            ColorMatrix clrMatrix = new(new float[][]
               {
            new float[] {-1, 0, 0, 0, 0},
            new float[] {0, -1, 0, 0, 0},
            new float[] {0, 0, -1, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {1, 1, 1, 0, 1}
               });

            using (ImageAttributes attrImage = new())
            {

                attrImage.SetColorMatrix(clrMatrix);

                using Graphics g = Graphics.FromImage(bmpDest);
                g.DrawImage(imgSource, new Rectangle(0, 0,
                imgSource.Width, imgSource.Height), 0, 0,
                imgSource.Width, imgSource.Height, GraphicsUnit.Pixel,
                attrImage);
            }

            return bmpDest;
        }
        internal static Image InvertGDI(Image imgSource)
        {
            Bitmap bmpDest;

            using (Bitmap bmpSource = new(imgSource))
            {
                bmpDest = new Bitmap(bmpSource.Width, bmpSource.Height);

                for (int x = 0; x < bmpSource.Width; x++)
                {
                    for (int y = 0; y < bmpSource.Height; y++)
                    {

                        Color clrPixel = bmpSource.GetPixel(x, y);

                        clrPixel = Color.FromArgb(255 - clrPixel.R, 255 -
                           clrPixel.G, 255 - clrPixel.B);

                        bmpDest.SetPixel(x, y, clrPixel);
                    }
                }
            }

            return bmpDest;

        }
    }
}
