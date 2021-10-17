using System;

namespace Buddhabrot
{
    class Program
    {
        static void Main(string[] args)
        {
            int size = 2000;
            int samples = 150 * 1000000;
            int threads = 10;

            // Generate fractals
            var fractalBuilder = new FractalBuilder();

            var r = new CountersArray(size);
            fractalBuilder.Counters = r;
            fractalBuilder.Build(samples, 1500, threads);

            var g = new CountersArray(size);
            fractalBuilder.Counters = g;
            fractalBuilder.Build(samples, 5000, threads);

            var b = new CountersArray(size);
            fractalBuilder.Counters = b;
            fractalBuilder.Build(samples, 9000, threads);

            // Save images
            var imageBuilder = new ImageBuilder(r, g, b);
            imageBuilder.SaveImage("D:\\buddhabrot.png");

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
