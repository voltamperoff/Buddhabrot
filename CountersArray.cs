using System;

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
            if (size < 1)
            {
                throw new ArgumentException("Size of the counters array must be at least 1 x 1");
            }

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
}
