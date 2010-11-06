using System;
using System.Collections.Generic;

using System.Text;

namespace Aont
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var Gnuplot = new Gnuplot(@"C:\Octave\3.2.0_gcc-4.3.0\bin\gnuplot.exe"))
            {
                //Gnuplot.Start();
                Gnuplot.ThrowErrorException = true;

                if (true)
                {
                    var Diffusion = new Diffusion(16, 16);
                    var Prob = Diffusion.Prob;
                    double X = Diffusion.X, Y = Diffusion.Y;

                    Gnuplot.WriteLine("set zrange [0:1]");
                    Gnuplot.ReadError();

                    Gnuplot.WriteLine("set hidden3d");
                    Gnuplot.ReadError();

                    //Gnuplot.WriteLine("set contour base");
                    //Gnuplot.WriteLine("set cntrparam levels 10");
                    //Gnuplot.WriteLine("unset surface");
                    //Gnuplot.WriteLine("set view 0,0");

                    //Gnuplot.WriteLine("set pm3d at s");
                    //Gnuplot.WriteLine("set palette rgbformulae 22,13,-31");
                    //Gnuplot.WriteLine("set cbrange[0:0.3]");
                    //Gnuplot.WriteLine("set pm3d");
                    while (true)
                    {
                        Gnuplot.WriteLine("splot '-'  w l");
                        //Console.WriteLine("splot");
                        for (int y = 0; y < Y; y++)
                        {
                            for (int x = 0; x < X; x++)
                            {
                                //Console.WriteLine("{0} {1} {2}", x, y, Prob[x, y]);
                                Gnuplot.WriteLine("{0} {1} {2}", x, y, Prob[x, y]);
                            }
                            Gnuplot.WriteLine();
                        }
                        Gnuplot.WriteLine("e");
                        Gnuplot.WriteLine();
                        Gnuplot.ReadError();
                        Prob = Diffusion.Next();
                    }
                }

                if (false)
                {
                    Gnuplot.WriteLine("set xrange [-3:3]"); Gnuplot.ReadError();
                    Gnuplot.WriteLine("set yrange [-3:3]"); Gnuplot.ReadError();
                    Gnuplot.WriteLine("set zrange [0:2]"); Gnuplot.ReadError();
                    Gnuplot.WriteLine("delta_t=0.1"); Gnuplot.ReadError();
                    Gnuplot.WriteLine("t=delta_t"); Gnuplot.ReadError();
                    while (true)
                    {
                        Gnuplot.WriteLine("t=t+delta_t"); Gnuplot.ReadError();
                        Gnuplot.WriteLine("splot exp(-(x**2+y**2)/(2*t))/sqrt(2*pi*t)"); Gnuplot.ReadError();
                    }
                }

                Console.ReadKey();
            }
        }
    }
}
