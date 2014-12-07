using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imagefilters
{
    public partial class Main : Form
    {
        private List<Filter> filters = new List<Filter>();
        private string path;// = Environment.CurrentDirectory + "\\flower.jpg";
        private WorkingImage image;


        public Main()
        {
            FileDialog d = new OpenFileDialog();
            if (d.ShowDialog() == DialogResult.OK)
                path = d.FileName;
            else
                Environment.Exit(0);

            Width = 800;
            Height = 600;

            filters.Add(new Filter(new double[,]
                {
                    {-1, -2, -1,},
                    {-2, 12, -2,},
                    {-1, -2, -1}
                }, "High Pass"));



            filters.Add(new Filter(new double[,]
                {
                    {  0,   0,   0, .01,   0,   0,   0,},
                    {  0,   0, .01, .01, .01,   0,   0,},
                    {  0, .01, .01, .01, .01, .01,   0,},
                    {.01, .01, .01, .01, .01, .01, .01,},
                    {  0, .01, .01, .01, .01, .01,   0,},
                    {  0,   0, .01, .01, .01,   0,   0,},
                    {  0,   0,   0, .01,   0,   0,   0,},
                }, "Bokeh"));

            filters.Add(new Filter(new double[,]
                {
                    { -.02, -.07, -.1, -.07, -.02,},
                    { -.07,  -.1, -.2,  -.1, -.07,},
                    {  -.1,  -.2,2.24,  -.2,  -.1,},
                    { -.07,  -.1, -.2,  -.1, -.07,},
                    { -.02, -.07, -.1, -.07, -.02,},
                }, "Edge HR"));

            filters.Add(new Filter(new double[,]
                {
                    { 0.0, 0.2, 0.0, },  
                    { 0.2, 0.2, 0.2, },   
                    { 0.0, 0.2, 0.2, },
                }, "Blur"));

            filters.Add(new Filter(new double[,]
                {
                    { .1, .1, .1, },  
                    { .1, .2, .1, },  
                    { .1, .1, .1, }

                }, "Soften"));

            filters.Add(new Filter(new double[,]
                { 
                    { 2,  0,  0, },  
                    { 0, -1,  0, },  
                    { 0,  0, -1, }
                }, "Emboss"));

            filters.Add(new Filter(new double[,]
                {
                    {0, -1, 0},
                    {-1, 5, -1},
                    {0, -1, 0}
                }, "Sharpen"));

            filters.Add(new Filter(new double[,]
                {
                    {-1, -1, -1},
                    {-1, 8, -1},
                    {-1, -1, -1}
                }, "Edge Detect"));

            image = new WorkingImage()
            {
                Name = "image",
                Dock = DockStyle.Fill
            };

            InitializeLayout();

            SetPicture(path);
        }

        private void InitializeLayout() 
        {
            TableLayoutPanel table = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 100));

            FlowLayoutPanel layout = new FlowLayoutPanel()
            {
                Name = "filterList",
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };

            layout.Controls.AddRange(filters.Select(i=> new Button()
            {
                Text = i.Name,
                Tag = i
            }).ToArray());

            foreach (Button control in layout.Controls)
            {
                control.Click += (sender, args) => ApplyFilter((Filter)((Button)sender).Tag);
            }

            table.Controls.Add(layout, 0, 0);

            table.Controls.Add(image, 1, 0);

            Controls.Add(table);

            SetPicture(path);
        }

        private void SetPicture(string path)
        {
            image.Load(path);
        }

        private void ApplyFilter(Filter f)
        {
            image.ApplyFilter(f);
            Invalidate();
        }
    }
}
