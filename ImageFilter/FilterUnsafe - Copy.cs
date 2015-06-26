using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace imagefilters
{
    public unsafe class FilterUnsafe
    {
        private readonly double[][] _filter;
        private string Name { get; set; }
        private int Height { get; set; }
        private int Width { get; set; }

        public FilterUnsafe(double[][] matrix, string name)
        {
            Height = matrix.Length;
            Width = matrix[0].Length;
            if (Height % 2 == 0 || Width % 2 == 0)
                throw new ArgumentException("Matrix must have odd number of rows and columns");

            this.Name = name;

            double total = 0;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    total += matrix[y][x];

            if(total>0)
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        matrix[y][x] = matrix[y][x] / total;

            _filter = matrix;

        }

        public unsafe Bitmap Apply(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            // Refer to 
            // http://msdn.microsoft.com/en-us/library/5ey6h79d.aspx
            // for locking info
            Rectangle bounds = new Rectangle(0, 0, width, height);

            BitmapData data = image.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            IntPtr pointer = data.Scan0;
            int bytes = Math.Abs(data.Stride) * height;

            byte[] imageBytes = new byte[bytes];
            byte[] resultBytes = new byte[bytes];

            Marshal.Copy(pointer, imageBytes, 0, bytes);

            image.UnlockBits(data);

            fixed (byte* pImage = imageBytes, pResult = resultBytes)
            {
                Convolute(pImage, pResult, width, height);
            }
            
            Bitmap result = new Bitmap(width, height);

            BitmapData resultData = result.LockBits(bounds,
                                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            Marshal.Copy(resultBytes, 0, resultData.Scan0, bytes);
            result.UnlockBits(resultData);
            
            return result;
        }

        private unsafe byte* Convolute(byte* imageBytes, byte* resultBytes, int width, int height)
        {

            fixed (double* pFilter = _filter[0])
            {

                int ht2 = (Height - 1)/2;
                int wd2 = (Width - 1)/2;

                double red = 0, green = 0, blue = 0;

                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int pos = Flatten(width, row, col);

                        for (int r = -ht2; r <= ht2; r++)
                        {
                            for (int c = -wd2; c <= wd2; c++)
                            {
                                if (row + r < 0 || row + r > height - 1
                                    || col + c < 0 || col + c > width - 1)
                                    continue;

                                int len = sizeof (double);
                                int x = r + ht2;
                                int y = c + wd2;
                                double p = *(pFilter + (x*len) + (y*len*Width));

                                red += FilterColor(imageBytes[Flatten(width, row + r, col + c)], p);
                                green += FilterColor(imageBytes[Flatten(width, row + r, col + c) + 1], p);
                                blue += FilterColor(imageBytes[Flatten(width, row + r, col + c) + 2], p);
                            }
                        }

                        resultBytes[pos] = (byte) Fix(red);
                        resultBytes[pos + 1] = (byte) Fix(green);
                        resultBytes[pos + 2] = (byte) Fix(blue);

                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                }
            }
            return resultBytes;
        }

        private unsafe int Flatten(int w, int y, int x)
        {
            return (y*w + x)*3;
        }

        private int Fix(double color)
        {
            return (int) (color > 255 ? 255 : color < 0 ? 0 : color);
        }

        private double FilterColor(int color2, double weight)
        {
            return color2*weight;
        }
    }
}
