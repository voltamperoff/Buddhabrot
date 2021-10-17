using System;
using System.Numerics;
using System.Threading;

namespace Buddhabrot
{
    class FractalBuilder
    {
        private static readonly Random random = new Random();
        private readonly object locker = new object();

        public Complex Power { get; set; } = new Complex(2.0, 0.0);
        public CountersArray Counters { get; set; }

        private Complex GetRandomPoint()
        {
            lock (locker)
            {
                double magnitude = Math.Sqrt(random.NextDouble()) * 2.0;
                double phase = random.NextDouble() * 2.0 * Math.PI;

                return Complex.FromPolarCoordinates(magnitude, phase);
            }
        }

        private bool IsPointConvergent(Complex point, int limit)
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

        private void CalculatePointPath(Complex point, int limit)
        {
            Complex z = new Complex(0.0, 0.0) + point;

            while (limit > 0)
            {
                // Map to counters array
                int x = Convert.ToInt32((z.Real + 2.0) / 4.0 * Counters.Size);
                int y = Convert.ToInt32((z.Imaginary + 2.0) / 4.0 * Counters.Size);

                if (x < 0 || x >= Counters.Size || y < 0 || y >= Counters.Size)
                {
                    break;
                }
                
                Counters.Increment(x, y);

                // Get next point position
                z = Complex.Pow(z, Power) + point;

                limit--;
            }
        }

        public void Build(int samplesCount, int limit, int threadsCount = 1)
        {
            if (samplesCount < 1)
            {
                throw new ArgumentException("At least 1 sample is needed");
            }

            if (limit < 1)
            {
                throw new ArgumentException("Limit cannot be less than 1");
            }

            if (threadsCount < 1)
            {
                throw new ArgumentException("Cannot run code on less than 1 thread");
            }

            if (Counters == null)
            {
                throw new InvalidOperationException("Counters array is not set up");
            }

            var threads = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    while (samplesCount / threadsCount > 0)
                    {
                        var point = GetRandomPoint();

                        if (!IsPointConvergent(point, limit))
                        {
                            CalculatePointPath(point, limit);
                            samplesCount--;
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
}
