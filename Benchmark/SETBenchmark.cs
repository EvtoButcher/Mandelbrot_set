using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Mandelbrot_set;

namespace Benchmark
{
    [MemoryDiagnoser]
    [RankColumn]

    public class SETBenchmark
    {
        private byte[] RGBarry = new byte[1499232];
        private int Width = 776;
        private int Height = 483;

        private readonly Form1 form = new Form1();

        [Benchmark]

        public void DotCheckWithCentr()
        {
            List<Point> points = new List<Point>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    points.Add(new Point(x, y));
                }
            }

            Parallel.ForEach(points, p =>
            {
                form.DotCheck(p.X, p.Y, RGBarry, Width, Height, 500);
            });
        }

        [Benchmark]

        public void DotCheckWithOutCentr()
        {
            List<Point> points = new List<Point>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    points.Add(new Point(x, y));
                }
            }

            Parallel.ForEach(points, p =>
            {
                //form.DotCheck_2(p.X, p.Y, RGBarry, Width, Height, 500);
            });
            
        }
    }
}
