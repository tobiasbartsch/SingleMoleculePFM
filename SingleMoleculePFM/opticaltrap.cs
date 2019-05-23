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
        private double _kx_command;
        private double _ky_command;
        private double _kz_command;
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
        private double _lowpassrate;
        private msequence _mymsequence;
        private bool _lowpass; // if lowpass = true, then low pass filter is on.
        private bool _pull; //if pull == true, then we are pulling. If false, we are relaxing
        private bool _mseq; // if mseq = true, run the mseq protocol
        private bool _ramp; // if ramp = true, run the ramp protocol
        

        public opticaltrap(double kx, double ky, double kz, double x, double y, double z)
        {
            _kx = kx;
            _ky = ky;
            _kz = kz;
            _kx_command = kx;
            _ky_command = ky;
            _kz_command = kz;
            _xcenter = x;
            _ycenter = y;
            _zcenter = z;
            _kx_ramp_low = 0; //initialize the lower value for force ramp experiments as zero
            _kx_ramp_high = kx; // initialize the upper value for force ramp experiments as kx
            _ky_ramp_low = 0; //initialize the lower value for force ramp experiments as zero
            _ky_ramp_high = ky; // initialize the upper value for force ramp experiments as kx
            _kz_ramp_low = 0; //initialize the lower value for force ramp experiments as zero
            _kz_ramp_high = kz; // initialize the upper value for force ramp experiments as kx
            _dkdt = 0; // no force ramp defined for the beginning.
            _lowpassrate = 0; // No low pass filter rate defined to begin
        }

        /// <summary>
        /// sets all spring constants to zero.
        /// </summary>
        public void TrapOff()
        {
            _kx = 0;
            _ky = 0;
            _kz = 0;
            _kx_command = 0;
            _ky_command = 0;
            _kz_command = 0;
        }


        /// <summary>
        /// Turns the low pass filter on, and sets a rate for the filter
        /// </summary>
        /// <param name="lowpassrate">Lowpassrate.</param>
        public void LowPassFilterOn(double lowpassrate)
        {
            _lowpass = true;
            _lowpassrate = lowpassrate;
        }



        /// <summary>
        /// Turns the pass filter off.
        /// </summary>
        public void LowPassFilterOff()
        {
            _lowpass = false;
            _lowpassrate = 0;
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
        public void InitSpringConstRamp(double kx_ramp_low, double kx_ramp_high, double ky_ramp_low, double ky_ramp_high, double kz_ramp_low, double kz_ramp_high, double dkdt)
        {
            _kx_ramp_low = kx_ramp_low; //initialize the lower value for force ramp experiments as zero
            _kx_ramp_high = kx_ramp_high; // initialize the upper value for force ramp experiments as kx
            _ky_ramp_low = ky_ramp_low; //initialize the lower value for force ramp experiments as zero
            _ky_ramp_high = ky_ramp_high; // initialize the upper value for force ramp experiments as kx
            _kz_ramp_low = kz_ramp_low; //initialize the lower value for force ramp experiments as zero
            _kz_ramp_high = kz_ramp_high; // initialize the upper value for force ramp experiments as kx
            _dkdt = dkdt;
            _pull = true;
            TrapOff();


            _ramp = true; // toggle the ramp condition on
            _mseq = false; // toggle the msequence condition off
        }



        /// <summary>
        /// Initialize a max length sequence. This switches the trap off (so that we commence pulling with zero force), sets the spring constants, but does not yet execute the M sequence.
        /// </summary>
        /// <param name="kx_ramp_low"></param>
        /// <param name="kx_ramp_high"></param>
        /// <param name="ky_ramp_low"></param>
        /// <param name="ky_ramp_high"></param>
        /// <param name="kz_ramp_low"></param>
        /// <param name="kz_ramp_high"></param>
        /// <param name="dkdt"></param>
        public void InitMaxLengthSequence(double kx_ramp_low, double kx_ramp_high, double ky_ramp_low, double ky_ramp_high, double kz_ramp_low, double kz_ramp_high, double lowpassfilterrate, msequence msequence)
        {
            _mymsequence = msequence;
            _kx_ramp_low = kx_ramp_low; //initialize the lower value for force ramp experiments as zero
            _kx_ramp_high = kx_ramp_high; // initialize the upper value for force ramp experiments as kx
            _ky_ramp_low = ky_ramp_low; //initialize the lower value for force ramp experiments as zero
            _ky_ramp_high = ky_ramp_high; // initialize the upper value for force ramp experiments as kx
            _kz_ramp_low = kz_ramp_low; //initialize the lower value for force ramp experiments as zero
            _kz_ramp_high = kz_ramp_high; // initialize the upper value for force ramp experiments as kx
            _pull = true;
            TrapOff();
            LowPassFilterOn(lowpassfilterrate); // toggle the low pass filter on
            

            _ramp = false; // toggle the ramp condition off
            _mseq = true; // toggle the msequence condition on

        }




        /// <summary>
        /// Propagates the Optical Trap forward by dt, and changes the corresponding spring values.
        /// </summary>
        /// <param name="dt">Dt.</param>
        public void PropagateOpticalTrap(double dt)
        {

            if (_ramp == true)
            {
                PropagateForceRamp(dt);
            }
            if (_mseq == true)
            {
                PropagateMSequence(dt);
            }

         
            if (_lowpass == true)
            {
                _kx += dt * _lowpassrate * (_kx_command - _kx);
                _ky += dt * _lowpassrate * (_ky_command - _ky);
                _kz += dt * _lowpassrate * (_kz_command - _kz);
            }
            else
            {
                _kx = _kx_command;
                _ky = _ky_command;
                _kz = _kz_command;
            }
        }



        /// <summary>
        /// Propagates forward the Msequence by dt, and changes the corresponding spring values. If low pass filter is turned on
        /// then the new value of _kx will be low pass filtered to the new commanded _kx_command. If not, it will be updated instantanously.
        /// </summary>
        /// <param name="current_MLS_val">Current mlsval.</param>
        private void PropagateMSequence(double dt)
        {

            // propgatage the msequence forward by dt
            _mymsequence.PropagateMsequenceFraction(dt);

           

            if (_mymsequence.value == 1)
            {
                _kx_command = _kx_ramp_high;
                _ky_command = _ky_ramp_high;
                _kz_command = _kz_ramp_high;
            }

            if (_mymsequence.value == -1)
            {
                _kx_command = _kx_ramp_low;
                _ky_command = _ky_ramp_low;
                _kz_command = _kz_ramp_low;
            }
        }


        /// <summary>
        /// Propagates the spring constant of the optical trap to perform force ramp experiments. You should call InitForceRamp before executing this for the first time or you will get unexpected results
        /// </summary>
        /// <param name="dt">time step in seconds</param>
        private void PropagateForceRamp(double dt)
        {
            //calculate kx, then scale other directions proportionally


            //are we ramping the force up or down?
            if(_pull==true)
            {
                //we are ramping up
                //make sure we do not ramp above kx_ramp_high
                if(_kx_command+dt*_dkdt > _kx_ramp_high)
                {
                    //stop pulling, start relaxing
                    _kx_command = _kx_command - dt * _dkdt;
                    _pull = false;
                }
                else
                {
                    //keep pulling
                    _kx_command = _kx_command + dt * _dkdt;
                }
            }
            else
            {
                //we are ramping down
                //make sure we do not ramp below kx_ramp_low
                if(_kx_command - dt * _dkdt < _kx_ramp_low)
                {
                    //stop relaxing, start pulling
                    _kx_command = _kx_command + dt * _dkdt;
                    _pull = true;
                }
                else
                {
                    //keep relaxing
                    _kx_command = _kx_command - dt * _dkdt;
                }
            }
            _ky_command = _kx_command / _kx_ramp_high * _ky_ramp_high;
            _kz_command = _kx_command / _kx_ramp_high * _kz_ramp_high;
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
            get 
            {
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
