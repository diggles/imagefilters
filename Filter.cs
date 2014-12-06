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
    public class Filter
    {
        private double[,] filter;
        public string Name { get; private set; }

        public Filter(double[,] matrix, string name)
        {
            if (matrix.GetLength(0) % 2 == 0 || matrix.GetLength(1) % 2 == 0)
                throw new ArgumentException("Matrix must have odd number of rows and columns");

            this.Name = name;

            double total = 0;
            for(int x = 0; x < matrix.GetLength(0); x++)
                for (int y = 0; y < matrix.GetLength(1); y++)
                    total += (double)matrix.GetValue(x, y);

            if(total>0)
                for (int x = 0; x < matrix.GetLength(0); x++)
                    for (int y = 0; y < matrix.GetLength(1); y++)
                        matrix.SetValue((double)matrix.GetValue(x, y)/total, x, y);

            filter = matrix;

        }

        public Bitmap Apply(Bitmap image)
        {
            // Refer to 
            // http://msdn.microsoft.com/en-us/library/5ey6h79d.aspx
            // for locking info
            Rectangle bounds = new Rectangle(0, 0, image.Width, image.Height);

            BitmapData data = image.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            IntPtr pointer = data.Scan0;
            int bytes  = Math.Abs(data.Stride) * image.Height;

            byte[] imageBytes = new byte[bytes];
            byte[] resultBytes = new byte[bytes];

            Marshal.Copy(pointer, imageBytes, 0, bytes);

            image.UnlockBits(data);

            int ht2 = (filter.GetLength(0) - 1) / 2;
            int wd2 = (filter.GetLength(1) - 1) / 2;

            double red = 0, green = 0, blue = 0;

            for (int row = 0; row < image.Height; row++)
            {
                for (int col = 0; col < image.Width; col++)
                {
                    int pos = Flatten(image.Width, row, col);

                    for (int r = -ht2; r <= ht2; r++)
                    {
                        for (int c = -wd2; c <= wd2; c++)
                        {
                            if (row + r < 0 || row + r > image.Height - 1
                                || col + c < 0 || col + c > image.Width - 1)
                                continue;

                            double p = filter[r + ht2, c + wd2];

                            red += FilterColor(imageBytes[Flatten(image.Width, row + r, col + c)], p);
                            green += FilterColor(imageBytes[Flatten(image.Width, row + r, col + c) + 1], p);
                            blue += FilterColor(imageBytes[Flatten(image.Width, row + r, col + c) + 2], p);
                        }
                    }

                    resultBytes[pos] = (byte)Fix(red);
                    resultBytes[pos + 1] = (byte)Fix(green);
                    resultBytes[pos + 2] = (byte)Fix(blue);
                    
                    red = 0;
                    green = 0;
                    blue = 0;
                }
            }

            Bitmap result = new Bitmap(image.Width, image.Height);

            BitmapData resultData = result.LockBits(bounds,
                                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            Marshal.Copy(resultBytes, 0, resultData.Scan0, bytes);
            result.UnlockBits(resultData);
            
            return result;
        }

        private int Flatten(int w, int y, int x)
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
