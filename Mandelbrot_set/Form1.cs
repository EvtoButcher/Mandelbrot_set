using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mandelbrot_set
{
    public partial class Form1 : Form
    {
        private Bitmap image;

        public Form1()
        {
            InitializeComponent();
            InitImage();
        }

        private void InitImage()
        {
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;
            Draw();
        }

        public void DotCheck(int x, int y, byte[] rgbValues, int Width, int Height, int n)
        {
            int Address = (y * Width + x) * 4;

            var xy0 = PointD.ToComplex(x, y, Width, Height);
            var xn = xy0.X;
            var yn = xy0.Y;
            double xn_1, yn_1;

            bool PointTrue = true;
            int itr = 0;

            
            double Pc, P, O;
            P = Math.Sqrt((x - 0.25) * (x - 0.25) + (y * y));
            O = Math.Atan2(y, x - 0.25);
            Pc = 0.5 - 0.5 * Math.Cos(O);

            if (P <= Pc)
            {
                rgbValues[Address] = 0;//B
                rgbValues[Address + 1] = 0;//G
                rgbValues[Address + 2] = 0;//R
                rgbValues[Address + 3] = 255; //alfa канал

            }
            else
            {
                for (; itr < n; itr++)
                {

                    xn_1 = xn * xn - yn * yn + xy0.X;
                    yn_1 = 2 * xn * yn + xy0.Y;

                    if (xn_1 * xn_1 + yn_1 * yn_1 > 4)
                    {
                        PointTrue = false;
                        break;
                    }
                    xn = xn_1;
                    yn = yn_1;
                }

                if (PointTrue)
                {
                    rgbValues[Address] = 0;//B
                    rgbValues[Address + 1] = 0;//G
                    rgbValues[Address + 2] = 0;//R
                    rgbValues[Address + 3] = 255; //alfa канал
                }
                else
                {
                    double k = (double)itr / n;//насколько точка далеко к множеству

                    rgbValues[Address] = (byte)(k * 75 + (1 - k) * 106);//B
                    rgbValues[Address + 1] = (byte)(k * 177 + (1 - k) * 158);//G
                    rgbValues[Address + 2] = (byte)(k * 255 + (1 - k) * 1);//R
                    rgbValues[Address + 3] = 255; //alfa канал
                }
            }
        }
        
        private void Draw()
        {
            int n = trackBar1.Value;//точность 

            var Data = image.LockBits(
                new Rectangle(0,0,pictureBox1.Width,pictureBox1.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );//делим по байтам все изображение

            IntPtr ptr = Data.Scan0;//адрес первого байта изображения

            int bytes = Math.Abs(Data.Stride) * pictureBox1.Height;//колличество байт в одной строке на колличество строк

            byte[] rgbValues = new byte[bytes];//массив изображения

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);//копируем изображение по байтам в rgbValues

            int Width = pictureBox1.Width;
            int Height = pictureBox1.Height;

            List <Point> points = new List<Point>(); 
            for (int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    points.Add(new Point(x, y));
                }
            }

            Parallel.ForEach(points, p =>
              {
                  DotCheck(p.X, p.Y, rgbValues, Width, Height, n);
              });

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);// переносим отредактированный массив в ptr

            image.UnlockBits(Data);

            pictureBox1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (PointD.scale > 200)
            {
                PointD.scale /= 1.1;
                Draw();
            }
            else
            {
                if (PointD.centr.X == 0 && PointD.centr.Y == 0)
                {
                    timer1.Stop();
                }
                else
                {
                    PointD.centr.X *= 1.5;
                    PointD.centr.Y *= 1.5;
                    Draw();
                }
                
            }
        }

        //интерфейс
        private void pictureBox1_SizeChanged(object sender, EventArgs e)//изминение разрешения в момент работы программы
        {
            InitImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PointD.centr = new PointD { X = 0, Y = 0 };
            PointD.scale = 200;
            Draw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var p = PointD.ToComplex(e.Location, pictureBox1.Width, pictureBox1.Height);

            label1.Text = $"X: {p.X}\nY: {p.Y}";
        }

        //функционал мыши

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var point = PointD.ToComplex(e.Location, pictureBox1.Width, pictureBox1.Height);

            PointD.scale *= e.Button == MouseButtons.Left ? 1.5 : 1.0 / 1.5;
            PointD.centr = e.Button == MouseButtons.Left ? point : PointD.centr;
            Draw();
        }
    }
}
