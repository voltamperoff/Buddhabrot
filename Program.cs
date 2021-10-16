using System;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace Buddhabrot
{
    class CountersArray
    {
        private readonly object locker = new object();
        private readonly int[,] counters;
        
        public int MaxCount { get; private set; }
        public int Size { get; private set; }

        public CountersArray(int size)
        {
            Size = size;

            counters = new int[Size, Size];
        }

        public void Increment(int x, int y)
        {
            lock (locker)
            {
                counters[x, y]++;

                if (MaxCount < counters[x, y])
                {
                    MaxCount = counters[x, y];
                }
            }
        }

        public double GetNormalizedValue(int x, int y)
        {
            return (double)counters[x, y] / (double)MaxCount;
        }
    }

    class Fractal
    {
        private static Random rnd = new Random();
        private static object randomLocker = new object();

        // Counters for storing dots frequency
        public CountersArray counters;
        public Complex Power = new Complex(2.0, 0.0);

        private Complex GetRandomPoint()
        {
            lock (randomLocker)
            {
                double magnitude = Math.Sqrt(rnd.NextDouble()) * 2.0;
                double phase = rnd.NextDouble() * 2.0 * Math.PI;

                return Complex.FromPolarCoordinates(magnitude, phase);
            }
        }

        private bool IsConvergent(Complex point, int limit)
        {
            Complex z = new Complex(0.0, 0.0) + point;

            while (limit > 0)
            {
                if (z.Magnitude >= 2.0)
                {
                    return false;
                }

                z = Complex.Pow(z, Power) + point;

                limit--;
            }

            return true;
        }

        private void Iterate(Complex point, int limit)
        {
            Complex z = new Complex(0.0, 0.0) + point;

            while (limit > 0)
            {
                // Map to counters array
                int x = Convert.ToInt32(((z.Real + 2.0) / 4.0) * counters.Size);
                int y = Convert.ToInt32(((z.Imaginary + 2.0) / 4.0) * counters.Size);

                if (x < 0 || x >= counters.Size || y < 0 || y >= counters.Size)
                {
                    break;
                }
                
                counters.Increment(x, y);

                // Get next number
                z = Complex.Pow(z, Power) + point;

                limit--;
            }
        }

        public void Generate(int iterationsCount, int limit, int threadsCount = 1)
        {
            var threads = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    while (iterationsCount / threadsCount > 0)
                    {
                        var point = GetRandomPoint();

                        if (!IsConvergent(point, limit))
                        {
                            Iterate(point, limit);
                            iterationsCount--;
                        }

                        if (iterationsCount % 1000 == 0)
                        {
                            Console.WriteLine($">> Iters: {iterationsCount}");
                        }
                    }
                });

                threads[i].Start();
            }

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i].Join();
            }
        }
    }

    class Image
    {
        public CountersArray R { get; set; }
        public CountersArray G { get; set; }
        public CountersArray B { get; set; }

        public void SaveImage(string path)
        {
            var size = R.Size;

            var image = new Bitmap(size, size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int r = Convert.ToInt32(255.0 * R.GetNormalizedValue(x, y));
                    int g = Convert.ToInt32(255.0 * G.GetNormalizedValue(x, y));
                    int b = Convert.ToInt32(255.0 * B.GetNormalizedValue(x, y));

                    image.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            image.Save(path);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int size = 2000;
            int samples = 50 * 1000000;

            var f = new Fractal();

            var r = new CountersArray(size);
            f.counters = r;
            f.Generate(samples, 100, 10);

            var g = new CountersArray(size);
            f.counters = g;
            f.Generate(samples, 500, 10);

            var b = new CountersArray(size);
            f.counters = b;
            f.Generate(samples, 1500, 10);

            var i = new Image();
            i.R = r;
            i.G = g;
            i.B = b;

            i.SaveImage("D:\\colored.png");

            Console.WriteLine("Hehe");
            Console.ReadLine();
        }
    }
}
