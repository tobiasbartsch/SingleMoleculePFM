using System.IO;
using System;
using SingleMoleculePFM.protein_models;
using SingleMoleculePFM.assays;

namespace SingleMoleculePFM
{
    /// <summary>
    /// Startup object containing the main class. Single-molecule assays are defined as static methods within this class; choose which one you want to call in the main method.
    /// </summary>
    class SimulateExperiment
    {
        //Define global parameters for our simulation
        static readonly double dx = 1e-11; //spatial resolution. 1e-10 m works well.
        static readonly double dtheta = 2e-5; //angular resolution in radians. For r = 500 nm of the probe bead, dtheta = 2e-4 results in 1e-10 m resolution of arc-length increments of the bead's surface
        static readonly double dt = 1e-7; //temporal resolution in seconds.
        static readonly Int64 N = (long)1e7; //number of points we wish to simulate.

        /// <summary>
        /// Call the definition of your assay from the main method. See 'double[,] timeseries = SimulateWLC();' as an example.
        /// </summary>
        static void Main(string[] args)
        {
            double[,] timeseries = SimulateWLC();

            StreamWriter file = new StreamWriter(@"C:\Users\tobia\Desktop\WLC.csv");
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

        /// <summary>
        /// Simulates motion of the probe bead on the end of a string of fixed length. Ignores all entropic influences of the string's conformations.
        /// </summary>
        /// <returns>timeseries: array of double of size [N, 6]. rows: simulated time steps, columns: x, y, and z coordinates of the probe's position.</returns>
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
            protein myprotein = new pcdh15(70e-9, 90e-9, 0.5e-9, 70e-9, 90e-9, 1e-1, 5e-1, 80e-9);
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

        static double[,] SimulateOne3nmStep()
        {
            protein myprotein = new One3nmStep(30e-9);
            particle myprobe = new particle(500e-9, 1, 1530e-9, 0e-9, 0e-9);
            pedestalbead mypedestal = new pedestalbead(1000e-9, 0e-9, 0, 0, 0);
            assay myassay = new DoubleBeadAssay(mypedestal, myprotein, myprobe, dx, dtheta);

            myprobe.ProteinAnchorAngles = new double[] { Math.PI / 2, Math.PI };

            opticaltrap maintrap = new opticaltrap(1e-5, 1e-5, 1e-6, 1500e-9, 0, 0);
            opticaltrap strongtrap = new opticaltrap(3e-4, 3e-4, 3e-5, 1700e-9, 0, 0);

            PFM mypfm = new PFM(myassay, maintrap, strongtrap);

            double[,] timeseries = mypfm.MakeTimeSeriesOfProbeMotionWithForceRamp(N, dt, 0.0, 3e-4, 0.0, 3e-4, 0.0, 3e-5, 0.01);

            return timeseries;
        }

        /// <summary>
        /// Simulates motion of the probe bead on the end of an inextensible WLC. Currently treats the WLC as "just" a pre-defined energy landscape so does not capture effects of different time scales of the chain..
        /// </summary>
        /// <returns>timeseries: array of double of size [N, 6]. rows: simulated time steps, columns: x, y, and z coordinates of the probe's position, angles theta and phi, and kx of the stimulus laser.</returns>
        static double[,] SimulateWLC()
        {
            protein myprotein = new WLC(5e-9, 50e-9);
            particle myprobe = new particle(500e-9, 1, 1540e-9, 0e-9, 0e-9);
            pedestalbead mypedestal = new pedestalbead(1000e-9, 0e-9, 0, 0, 0);
            assay myassay = new DoubleBeadAssay(mypedestal, myprotein, myprobe, dx, dtheta);

            myprobe.ProteinAnchorAngles = new double[] { Math.PI / 2, Math.PI };

            opticaltrap maintrap = new opticaltrap(1e-5, 1e-5, 1e-6, 1500e-9, 0, 0);
            opticaltrap strongtrap = new opticaltrap(3e-4, 3e-4, 3e-5, 1700e-9, 0, 0);

            PFM mypfm = new PFM(myassay, maintrap, strongtrap);

            double[,] timeseries = mypfm.MakeTimeSeriesOfProbeMotionWithForceRamp(N, dt, 0.0, 3.5e-4, 0.0, 3.5e-4, 0.0, 3.5e-5, 8e-4);

            return timeseries;
        }

        /// <summary>
        /// Simulates motion of the probe bead on the end of an (almost) inextensible stick.
        /// </summary>
        /// <returns>timeseries: array of double of size [N, 6]. rows: simulated time steps, columns: x, y, and z coordinates of the probe's position, angles theta and phi, and kx of the stimulus laser.</returns>
        static double[,] SimulateStick()
        {
            protein myprotein = new StickProtein(50e-9);
            particle myprobe = new particle(500e-9, 1, 1550e-9, 0e-9, 0e-9);
            pedestalbead mypedestal = new pedestalbead(1000e-9, 0e-9, 0, 0, 0);
            assay myassay = new DoubleBeadAssay(mypedestal, myprotein, myprobe, dx, dtheta);

            myprobe.ProteinAnchorAngles = new double[] { Math.PI / 2, Math.PI };

            opticaltrap maintrap = new opticaltrap(1e-5, 1e-5, 1e-6, 1500e-9, 0, 0);
            opticaltrap strongtrap = new opticaltrap(0, 0, 0, 1700e-9, 0, 0);

            PFM mypfm = new PFM(myassay, maintrap, strongtrap);

            //double[,] timeseries = mypfm.MakeTimeSeriesOfProbeMotionWithForceRamp(N, dt, 0.0, 3.5e-4, 0.0, 3.5e-4, 0.0, 3.5e-5, 8e-4);
            double[,] timeseries = mypfm.MakeTimeSeriesOfProbeMotion(N, dt);

            return timeseries;
        }


    }
}
