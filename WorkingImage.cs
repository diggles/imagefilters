using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imagefilters
{
    public class WorkingImage : Control
    {
        private Bitmap original;
        private Bitmap filtered;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (original == null) return;
            e.Graphics.DrawImage(filtered??original, 0, 0, Width, Height);    
        }

        public void ApplyFilter(Filter filter)
        {
            filtered = filter.Apply(filtered??original);
            Invalidate();
        }

        public void Load(string fileName)
        {
            filtered = null;
            try
            {
                using (Stream bitmapStream = File.Open(fileName, FileMode.Open))
                {
                    Image img = Image.FromStream(bitmapStream);
                    original = new Bitmap(img);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
