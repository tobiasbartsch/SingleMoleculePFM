using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    class opticaltrap
    {
        private double _kx;
        private double _ky;
        private double _kz;
        private double _xcenter;
        private double _ycenter;
        private double _zcenter;
        private double _kx_ramp_low;
        private double _kx_ramp_high;
        private double _ky_ramp_low;
        private double _ky_ramp_high;
        private double _kz_ramp_low;
        private double _kz_ramp_high;
        private double _dkdt;
        private bool _pull; //if pull == true, then we are pulling. If false, we are relaxing

        public opticaltrap(double kkx, double kky, double kkz, double x, double y, double z)
        {
            _kx = kkx;
            _ky = kky;
            _kz = kkz;
            _xcenter = x;
            _ycenter = y;
            _zcenter = z;
            _kx_ramp_low = 0; //initialize the lower value for force ramp experiments as zero
            _kx_ramp_high = kkx; // initialize the upper value for force ramp experiments as kkx
            _ky_ramp_low = 0; //initialize the lower value for force ramp experiments as zero
            _ky_ramp_high = kky; // initialize the upper value for force ramp experiments as kkx
            _kz_ramp_low = 0; //initialize the lower value for force ramp experiments as zero
            _kz_ramp_high = kkz; // initialize the upper value for force ramp experiments as kkx
            _dkdt = 0; // no force ramp defined for the beginning.
        }

        /// <summary>
        /// sets all spring constants to zero.
        /// </summary>
        public void TrapOff()
        {
            _kx = 0;
            _ky = 0;
            _kz = 0;
        }

        /// <summary>
        /// Initialize a force ramp experiment. This switches the trap off (so that we commence pulling with zero force), sets the spring constants, but does not yet execute pulling.
        /// </summary>
        /// <param name="kx_ramp_low"></param>
        /// <param name="kx_ramp_high"></param>
        /// <param name="ky_ramp_low"></param>
        /// <param name="ky_ramp_high"></param>
        /// <param name="kz_ramp_low"></param>
        /// <param name="kz_ramp_high"></param>
        /// <param name="dkdt"></param>
        public void InitForceRamp(double kx_ramp_low, double kx_ramp_high, double ky_ramp_low, double ky_ramp_high, double kz_ramp_low, double kz_ramp_high, double dkdt)
        {
            _kx_ramp_low = kx_ramp_low; //initialize the lower value for force ramp experiments as zero
            _kx_ramp_high = kx_ramp_high; // initialize the upper value for force ramp experiments as kkx
            _ky_ramp_low = ky_ramp_low; //initialize the lower value for force ramp experiments as zero
            _ky_ramp_high = ky_ramp_high; // initialize the upper value for force ramp experiments as kkx
            _kz_ramp_low = kz_ramp_low; //initialize the lower value for force ramp experiments as zero
            _kz_ramp_high = kz_ramp_high; // initialize the upper value for force ramp experiments as kkx
            _dkdt = dkdt; // no force ramp defined for the beginning.
            _pull = true;
            TrapOff();
        }

        /// <summary>
        /// Propagates the spring constant of the optical trap to perform force ramp experiments. You should call InitForceRamp before executing this for the first time or you will get unexpected results
        /// </summary>
        /// <param name="dt">time step in seconds</param>
        public void PropagateForceRamp(double dt)
        {
            //calculate kx, then scale other directions proportionally
            
            //are we ramping the force up or down?
            if(_pull==true)
            {
                //we are ramping up
                //make sure we do not ramp above kx_ramp_high
                if(_kx+dt*_dkdt > _kx_ramp_high)
                {
                    //stop pulling, start relaxing
                    _kx = _kx - dt * _dkdt;
                    _pull = false;
                }
                else
                {
                    //keep pulling
                    _kx = _kx + dt * _dkdt;
                }
            }
            else
            {
                //we are ramping down
                //make sure we do not ramp below kx_ramp_low
                if(_kx - dt * _dkdt < _kx_ramp_low)
                {
                    //stop relaxing, start pulling
                    _kx = _kx + dt * _dkdt;
                    _pull = true;
                }
                else
                {
                    //keep relaxing
                    _kx = _kx - dt * _dkdt;
                }
            }
            _ky = _kx / _kx_ramp_high * _ky_ramp_high;
            _kz = _kx / _kx_ramp_high * _kz_ramp_high;
        }

        /// <summary>
        /// Potential energy of a particle in the optical trap if the particle is at global coordinates x, y, z
        /// </summary>
        /// <param name="x">x coordinate of the particle, global coordinate system</param>
        /// <param name="y">y coordinate of the particle, global coordinate system</param>
        /// <param name="z">z coordinate of the particle, global coordinate system</param>
        /// <returns>potential energy of the trapped particle</returns>
        public double TrapEnergy(double x, double y, double z)
        {
            double energy = 0.5 * (_kx * Math.Pow(x - _xcenter, 2) + _ky * Math.Pow(y - _ycenter, 2) + _kz * Math.Pow(z - _zcenter, 2));
            return energy;
        }

        /// <summary>
        /// Forces acting on the trapped particle.
        /// </summary>
        /// <param name="x">x coordinate of the particle, global coordinate system</param>
        /// <param name="y">y coordinate of the particle, global coordinate system</param>
        /// <param name="z">z coordinate of the particle, global coordinate system</param>
        /// <param name="dx">dx to be used to calculate force from energy/param>
        /// <returns>array of forces acting on the particle</returns>
        public double[] TrapForce(double x, double y, double z, double dx)
        {
            double[] forces = new double[3];
            forces[0] = -(TrapEnergy(x + dx, y, z) - TrapEnergy(x, y, z)) / dx;
            forces[1] = -(TrapEnergy(x, y + dx, z) - TrapEnergy(x, y, z)) / dx;
            forces[2] = -(TrapEnergy(x, y, z + dx) - TrapEnergy(x, y, z)) / dx;
            return forces;
        }

        /// <summary>
        /// spring constant along x direction
        /// </summary>
        public double kx {
            get {
                return _kx;
            }
            set
            {
                _kx = kx;
            }
        }

        /// <summary>
        /// spring constant along y direction
        /// </summary>
        public double ky
        {
            get
            {
                return _ky;
            }
            set
            {
                _ky = ky;
            }
        }

        /// <summary>
        /// spring constant along z direction
        /// </summary>
        public double kz
        {
            get
            {
                return _kz;
            }
            set
            {
                _kz = kz;
            }
        }
    }
}
