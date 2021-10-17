using System;
using System.Drawing;

namespace Buddhabrot
{
    class ImageBuilder
    {
        private CountersArray countersR;
        private CountersArray countersG;
        private CountersArray countersB;

        public ImageBuilder(CountersArray r, CountersArray g, CountersArray b)
        {
            if (r == null || g == null || b == null)
            {
                throw new ArgumentNullException("Counters array is not initialized");
            }

            if (r.Size != g.Size || r.Size != b.Size)
            {
                throw new ArgumentException("Counters arrays must be same size");
            }

            countersR = r;
            countersG = g;
            countersB = b;
        }

        public void SaveImage(string path)
        {
            var size = countersR.Size;

            var image = new Bitmap(size, size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int r = Convert.ToInt32(255.0 * countersR.GetNormalizedValue(x, y));
                    int g = Convert.ToInt32(255.0 * countersG.GetNormalizedValue(x, y));
                    int b = Convert.ToInt32(255.0 * countersB.GetNormalizedValue(x, y));

                    image.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            image.Save(path);
        }
    }
}
