using System.Drawing;

namespace Mandelbrot_set
{
    class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static double scale = 200;

        public static PointD centr = new PointD { X = 0, Y = 0 };

        public static PointD ToComplex(Point point, int Width, int Height)
        {
            return ToComplex(point.X, point.Y, Width, Height);
        }

        public static PointD ToComplex(int x, int y, int Width, int Height)
        {
            return new PointD
            {
                X = (double)(x - (double)Width / 2) / scale + centr.X,
                Y = -(double)(y - (double)Height / 2) / scale + centr.Y,
            };
        }
    }
}
