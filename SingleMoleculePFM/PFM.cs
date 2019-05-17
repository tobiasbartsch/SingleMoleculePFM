using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleMoleculePFM.assays;

namespace SingleMoleculePFM
{
    class PFM
    {
        private assay _myassay;
        private opticaltrap _mephistotrap;
        private opticaltrap _strongtrap;

        /// <summary>
        /// Makes a PFM consisting of an assay and two optical traps.
        /// </summary>
        /// <param name="myassay">Single molecule assay</param>
        /// <param name="mephistotrap">Optical trap formed by the Mephisto</param>
        /// <param name="strongtrap">Optical trap formed by the strong laser. If you do not want to use a strong laser, just set its spring constant to zero.</param>
        public PFM(assay myassay, opticaltrap mephistotrap, opticaltrap strongtrap)
        {
            _myassay = myassay;
            _mephistotrap = mephistotrap;
            _strongtrap = strongtrap;
        }

        /// <summary>
        /// computes the total forces acting on the linear motion of the bead, consisting of forces from the assay and forces from each optical trap
        /// </summary>
        private double[] TotalForcesLinMotion
        {
            get
            {
                double[] totalforces = new double[3];
                int i = 0;
                for(i=0;i<3;i++)
                {
                    totalforces[i] = _myassay.forcesOnLinMotion[i] + _mephistotrap.TrapForce(_myassay.probe.position[0], _myassay.probe.position[1], _myassay.probe.position[2], _myassay.dx)[i] + _strongtrap.TrapForce(_myassay.probe.position[0], _myassay.probe.position[1], _myassay.probe.position[2], _myassay.dx)[i];
                }
                if(totalforces[0]<-1e-10)
                {
                    Console.WriteLine("########");
                    Console.WriteLine(_myassay.forcesOnLinMotion[0]);
                    Console.WriteLine(_mephistotrap.TrapForce(_myassay.probe.position[0], _myassay.probe.position[1], _myassay.probe.position[2], _myassay.dx)[0]);
                    Console.WriteLine(_strongtrap.TrapForce(_myassay.probe.position[0], _myassay.probe.position[1], _myassay.probe.position[2], _myassay.dx)[0]);
                    Console.WriteLine("########");
                }
                return totalforces;
            }
        }

        /// <summary>
        /// computes the total forces acting on the rotation of the bead. For now that is only the assay, the optical traps do not influence rotation
        /// </summary>
        private double[] TotalForcesRotMotion
        {
            get
            {
                return _myassay.forcesOnRotMotion;
            }
        }

        /// <summary>
        /// Simulates a time series of probe motion
        /// </summary>
        /// <param name="N"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double[,] MakeTimeSeriesOfProbeMotion(long N, double dt)
        {
            double[,] timeseries = new double[N,6];

            int i = 0;
            for(i=0;i<N;i++)
            {
                Console.WriteLine(i);
                timeseries[i, 0] = _myassay.probe.position[0];
                timeseries[i, 1] = _myassay.probe.position[1];
                timeseries[i, 2] = _myassay.probe.position[2];
                timeseries[i, 3] = _myassay.probe.angles[0];
                timeseries[i, 4] = _myassay.probe.angles[1];
                timeseries[i, 5] = _strongtrap.kx;
                _myassay.probe.PropagatePosition(dt, TotalForcesLinMotion, _myassay);
                _myassay.probe.PropagateRotation(dt, TotalForcesRotMotion, _myassay);
            }

            return timeseries;
        }

        /// <summary>
        /// Simulates a time series of probe motion with force ramp from strong trap
        /// </summary>
        /// <param name="N">number of points in the time series</param>
        /// <param name="dt">time step in seconds</param>
        /// <param name="kx_ramp_low">lower bound of kx</param>
        /// <param name="kx_ramp_high">upper bound of kx</param>
        /// <param name="ky_ramp_low">lower bound of ky</param>
        /// <param name="ky_ramp_high">upper bound of ky</param>
        /// <param name="kz_ramp_low">lower bound of kz</param>
        /// <param name="kz_ramp_high">upper bound of kz</param>
        /// <param name="dkdt">rate at which the spring constant of the strong trap is modulated</param>
        /// <returns></returns>
        public double[,] MakeTimeSeriesOfProbeMotionWithForceRamp(long N, double dt, double kx_ramp_low, double kx_ramp_high, double ky_ramp_low, double ky_ramp_high, double kz_ramp_low, double kz_ramp_high, double dkdt)
        {
            _strongtrap.InitSpringConstRamp(kx_ramp_low, kx_ramp_high, ky_ramp_low, ky_ramp_high, kz_ramp_low, kz_ramp_high, dkdt);

            double[,] timeseries = new double[N, 6];
            double unfoldingprob = 0.0;
            int i = 0;
            for (i = 0; i < N; i++)
            {
                Console.WriteLine(i);
                timeseries[i, 0] = _myassay.probe.position[0];
                timeseries[i, 1] = _myassay.probe.position[1];
                timeseries[i, 2] = _myassay.probe.position[2];
                timeseries[i, 3] = _myassay.probe.angles[0];
                timeseries[i, 4] = _myassay.probe.angles[1];
                timeseries[i, 5] = _strongtrap.kx;

                //if the forces are very high we have to slow down time to not catapult the particle. Piconewtons are fine at our usual speed. Nanonewtons are not!
                if(Math.Abs(TotalForcesLinMotion[0]/1e-12) > 10)
                {
                    //slow down time
                    int j = 0;
                    int slowdown = (int)Math.Abs(TotalForcesLinMotion[0] / 1e-12) * 10;
                    for (j = 0; j < slowdown; j++)
                    {
                        _myassay.probe.PropagatePosition(dt/slowdown, TotalForcesLinMotion, _myassay);
                        _myassay.probe.PropagateRotation(dt/slowdown, TotalForcesRotMotion, _myassay);
                        //_myassay.protein.PropagateFolding(_strongtrap.TrapForce(_myassay.probe.position[0], _myassay.probe.position[1], _myassay.probe.position[2], _myassay.dx)[0], dt/slowdown, _myassay.dx);
                        _strongtrap.PropagateForceRamp(dt/slowdown);
                    }

                }
                else
                {
                    //regular time
                    _myassay.probe.PropagatePosition(dt, TotalForcesLinMotion, _myassay);
                    _myassay.probe.PropagateRotation(dt, TotalForcesRotMotion, _myassay);
                    //_myassay.protein.PropagateFolding(TotalForcesLinMotion[0], dt, _myassay.dx);
                    _strongtrap.PropagateForceRamp(dt);
                }
            }

            return timeseries;
        }





        public double[,] MakeTimeSeriesOfProbeMotionWithMSequence(long N, double dt, double kx_ramp_low, double kx_ramp_high, double ky_ramp_low, double ky_ramp_high, double kz_ramp_low, double kz_ramp_high, double dkdt)
        {

            _strongtrap.InitMaxLengthSequence(kx_ramp_low, kx_ramp_high, ky_ramp_low, ky_ramp_high, kz_ramp_low, kz_ramp_high);


            // Initiate and load the msequence from a file
            msequence _mymsequence = new msequence(@"/Users/dfirester1/maxlengthsequence.txt",1e-5);
            
            double[,] timeseries = new double[N, 6];
            double unfoldingprob = 0.0;
            int i = 0;
            for (i = 0; i < N; i++)
            {
                Console.WriteLine(i);
                timeseries[i, 0] = _myassay.probe.position[0];
                timeseries[i, 1] = _myassay.probe.position[1];
                timeseries[i, 2] = _myassay.probe.position[2];
                timeseries[i, 3] = _myassay.probe.angles[0];
                timeseries[i, 4] = _myassay.probe.angles[1];
                timeseries[i, 5] = _strongtrap.kx;

              

                //if the forces are very high we have to slow down time to not catapult the particle. Piconewtons are fine at our usual speed. Nanonewtons are not!
                if (Math.Abs(TotalForcesLinMotion[0] / 1e-12) > 10)
                {
                    //slow down time
                    int j = 0;
                    int slowdown = (int)Math.Abs(TotalForcesLinMotion[0] / 1e-12) * 10;
                    for (j = 0; j < slowdown; j++)
                    {
                        _myassay.probe.PropagatePosition(dt / slowdown, TotalForcesLinMotion, _myassay);
                        _myassay.probe.PropagateRotation(dt / slowdown, TotalForcesRotMotion, _myassay);
                        //_myassay.protein.PropagateFolding(_strongtrap.TrapForce(_myassay.probe.position[0], _myassay.probe.position[1], _myassay.probe.position[2], _myassay.dx)[0], dt/slowdown, _myassay.dx);
                        _mymsequence.AdvanceMsequenceFraction(dt / (_mymsequence.dt_mseq*slowdown));


                        _strongtrap.UpdateSpringValue(_mymsequence.value);
                    }

                }
                else
                {
                    //regular time
                    _myassay.probe.PropagatePosition(dt, TotalForcesLinMotion, _myassay);
                    _myassay.probe.PropagateRotation(dt, TotalForcesRotMotion, _myassay);
                    //_myassay.protein.PropagateFolding(TotalForcesLinMotion[0], dt, _myassay.dx);
                    _mymsequence.AdvanceMsequenceFraction(dt / _mymsequence.dt_mseq);


                    _strongtrap.UpdateSpringValue(_mymsequence.value);
                }
            }

            return timeseries;
        }

    }
}



