using System;
using System.Collections.Generic;
using System.Text;

namespace Aont
{
    class Diffusion
    {
        public readonly int X, Y;
        public double Diff = 0.001;
        public double[,] Prob;
        public Diffusion(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.Prob = new double[X, Y];
            this.Prob[X / 2, Y / 2] = 1;
        }
        public double[,] Next()
        {
            var NextProb = new double[X, Y];
            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    NextProb[x, y] = Prob[x, y]
                        + Diff * (PeriodicProb(x + 1, y) + PeriodicProb(x - 1, y) + PeriodicProb(x, y + 1) + PeriodicProb(x, y - 1) - 4 * Prob[x, y]);
                }
            }
            this.Prob = NextProb;
            return Prob;
        }
        double PeriodicProb(int x, int y)
        {
            while (x < 0) x += X;
            while (x >= X) x -= X;
            while (y < 0) y += Y;
            while (y >= Y) y -= Y;
            return Prob[x, y];
        }
    }
}
