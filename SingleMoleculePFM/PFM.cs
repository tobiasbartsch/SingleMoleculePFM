using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            double[,] timeseries = new double[N,3];

            int i = 0;
            for(i=0;i<N;i++)
            {
                Console.WriteLine(i);
                timeseries[i, 0] = _myassay.probe.position[0];
                timeseries[i, 1] = _myassay.probe.position[1];
                timeseries[i, 2] = _myassay.probe.position[2];
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
            _strongtrap.InitForceRamp(kx_ramp_low, kx_ramp_high, ky_ramp_low, ky_ramp_high, kz_ramp_low, kz_ramp_high, dkdt);

            double[,] timeseries = new double[N, 4];

            int i = 0;
            for (i = 0; i < N; i++)
            {
                Console.WriteLine(i);
                timeseries[i, 0] = _myassay.probe.position[0];
                timeseries[i, 1] = _myassay.probe.position[1];
                timeseries[i, 2] = _myassay.probe.position[2];
                timeseries[i, 3] = _strongtrap.kx;
                _myassay.probe.PropagatePosition(dt, TotalForcesLinMotion, _myassay);
                _myassay.probe.PropagateRotation(dt, TotalForcesRotMotion, _myassay);
                _strongtrap.PropagateForceRamp(dt);
            }

            return timeseries;
        }

    }
}
