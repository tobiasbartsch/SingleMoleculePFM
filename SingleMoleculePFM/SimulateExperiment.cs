using System.IO;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    class SimulateExperiment
    {
        static double dx = 1e-9;
        static double dtheta = 2e-3;

        static double dt = 10e-6;
        static Int64 N = 4000000;

        static void Main(string[] args)
        {
            double[,] timeseries = SimulatePCDH15();

            StreamWriter file = new StreamWriter(@"C:\Users\tobia\Desktop\test.csv");
            //my2darray  is my 2d array created.
            Console.WriteLine(timeseries.GetLength(0));
            Console.WriteLine(timeseries.GetLength(1));
            for (int j = 0; j < timeseries.GetLength(0); j++)
            {
                for (int i = 0; i < timeseries.GetLength(1); i++)
                {
                    file.Write(timeseries[j, i]);
                    file.Write(",");
                }
                file.Write("\n"); // go to next line
            }
        }

        static double[,] SimulateStringProtein()
        {
            protein myprotein = new stringprotein(50e-9);
            particle myprobe = new particle(500e-9, 1, 1520e-9, 0e-9, 0e-9);
            pedestalbead mypedestal = new pedestalbead(1000e-9, 0e-9, 0, 0, 0);
            assay myassay = new DoubleBeadAssay(mypedestal, myprotein, myprobe, dx, dtheta);
        
            myprobe.ProteinAnchorAngles = new double[] { Math.PI / 2, Math.PI};

            opticaltrap maintrap = new opticaltrap(1e-5, 1e-5, 1e-6, 1500e-9, 0, 0);
            opticaltrap strongtrap = new opticaltrap(0, 0, 0, 1500e-9, 0, 0);

            PFM mypfm = new PFM(myassay, maintrap, strongtrap);

            double[,] timeseries = mypfm.MakeTimeSeriesOfProbeMotion(N, dt);

            return timeseries;
        }

        static double[,] SimulatePCDH15()
        {
            protein myprotein = new pcdh15(40e-9, 160e-5, 100e-9, 20e-5, 55e-9);
            particle myprobe = new particle(500e-9, 1, 1520e-9, 0e-9, 0e-9);
            pedestalbead mypedestal = new pedestalbead(1000e-9, 0e-9, 0, 0, 0);
            assay myassay = new DoubleBeadAssay(mypedestal, myprotein, myprobe, dx, dtheta);

            myprobe.ProteinAnchorAngles = new double[] { Math.PI / 2, Math.PI };

            opticaltrap maintrap = new opticaltrap(1e-5, 1e-5, 1e-6, 1500e-9, 0, 0);
            opticaltrap strongtrap = new opticaltrap(3e-4, 3e-4, 3e-5, 1700e-9, 0, 0);

            PFM mypfm = new PFM(myassay, maintrap, strongtrap);

            double[,] timeseries = mypfm.MakeTimeSeriesOfProbeMotionWithForceRamp(N, dt, 0.0,3e-4,0.0, 3e-4, 0.0, 3e-5, 0.001 );

            return timeseries;
        }

    }
}
