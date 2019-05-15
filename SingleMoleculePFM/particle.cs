using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleMoleculePFM.assays;

namespace SingleMoleculePFM
{
    /// <summary>
    /// Defines a probe particle
    /// </summary>
    class particle
    {
        private double _R;
        private double _charge;
        private double _x;
        private double _y;
        private double _z;
        private double _theta;
        private double _phi;
        private double _drag;
        private double _dragrot;
        /// <summary>
        /// anchor angle theta of the protein, in the coordinate system of the bead
        /// </summary>
        private double _anchortheta;
        /// <summary>
        /// anchor angle phi of the protein, in the coordinate system of the bead
        /// </summary>
        private double _anchorphi;
        private Random _rng;

        public particle(double RR, double ccharge, double xx, double yy, double zz)
        {
            _R = RR;
            _charge = ccharge;
            _x = xx;
            _y = yy;
            _z = zz;
            _rng = new Random();
            _drag = 6 * Math.PI * constants.eta * _R;
            _dragrot = 8 * Math.PI * constants.eta * Math.Pow(_R, 3);
            _anchortheta = Math.PI / 2;
            _anchorphi = -Math.PI;
            _theta = 0;
            _phi = 0;
        }

        /// <summary>
        /// propagates the position of the particle, moving forward in time by dt
        /// </summary>
        /// <param name="dt">length of time step to advance time trace by</param>
        /// <param name="localforcesPos">array of perpendicular forces acting on the particle in x, y, and z direction</param>
        /// <param name="myassay">assay that describes how (and if) the particle is anchored</param>
        public void PropagatePosition(double dt, double[] localforcesPos, assay myassay)
        {
            double x0 = _x;
            double y0 = _y;
            double z0 = _z;

            // keep trying to advance to position until we end up somewhere that does NOT have a positive infinite energy.
            int counter = 0;
            do
            {
                //if (double.IsPositiveInfinity(myassay.energy))
                //{
                //    if(localforcesPos[0] < -1e-10)
                //    {
                //        Console.WriteLine("blah2");
                //    }
                //    Console.WriteLine("from: " + x0 + " with force: " + localforcesPos[0]);
                //    Console.WriteLine("assay force: " + myassay.forcesOnLinMotion[0]);
                //}
                counter += 1;
                //if(counter > 100) {
                //    Console.WriteLine(counter);
                //}
                _x = x0;
                _y = y0;
                _z = z0;
                _x += Math.Sqrt(2 * D * dt) * _rng.NextGaussian() + dt * localforcesPos[0] / _drag;
                _y += Math.Sqrt(2 * D * dt) * _rng.NextGaussian() + dt * localforcesPos[1] / _drag;
                _z += Math.Sqrt(2 * D * dt) * _rng.NextGaussian() + dt * localforcesPos[2] / _drag;
            } while (double.IsPositiveInfinity(myassay.energy));
        }

        /// <summary>
        /// propagates the rotation of the particle, moving forward in time by dt
        /// </summary>
        /// <param name="dt">length of time step to advance time trace by</param>
        /// <param name="localforcesAngle">array of tangential forces acting on the particle in theta (inclination) and phi (azimuth) direction</param>
        /// <param name="myassay">assay that describes how (and if) the particle is anchored</param>
        public void PropagateRotation(double dt, double[] localforcesAngle, assay myassay)
        {
            double theta0 = _theta;
            double phi0 = _phi;
            // keep trying to advance the rotation until we end up somewhere that does NOT have a positive infinite energy.
            do
            {
                _theta = theta0;
                _phi = phi0;
                _theta += Math.Sqrt(2 * Drot * dt) * _rng.NextGaussian() + dt * R * localforcesAngle[0] / _dragrot;
                _phi += Math.Sqrt(2 * Drot * dt) * _rng.NextGaussian() + dt * R * localforcesAngle[1] / _dragrot;
            } while (double.IsPositiveInfinity(myassay.energy));
        }

        /// <summary>
        /// gets and sets the ProteinAnchorAngles in the coordinate system of the BEAD {anchortheta, anchorphi}. These angles will rotate with the bead.
        /// </summary>
        public double[] ProteinAnchorAngles
        {
            get
            {
                double[] protanchors = new double[2];
                protanchors[0] = _anchortheta;
                protanchors[1] = _anchorphi;
                return protanchors;
            }
            set
            {
                _anchortheta = ProteinAnchorAngles[0];
                _anchorphi = ProteinAnchorAngles[1];
            }
        }

        /// <summary>
        /// particle radius
        /// </summary>
        public double R
        {
            get
            {
                return _R;
            }
        }

        /// <summary>
        /// diffusion constant
        /// </summary>
        public double D
        {
            get
            {
                return constants.kB* constants.T / (6 * Math.PI * constants.eta * _R);
            }
        }

        /// <summary>
        /// rotational diffusion constant
        /// </summary>
        public double Drot
        {
            get
            {
                return constants.kB * constants.T / (8 * Math.PI * constants.eta * Math.Pow(_R, 3));
            }
        }

        /// <summary>
        /// charge of the particle
        /// </summary>
        public double charge
        {
            get
            {
                return _charge;
            }
        }

        /// <summary>
        /// position of the center of the particle
        /// </summary>
        public double[] position
        {
            get
            {
                double[] pposition = new double[3];
                pposition[0] = _x;
                pposition[1] = _y;
                pposition[2] = _z;
                return pposition;
            }
        }

        /// <summary>
        /// the angular orientation of the bead. {theta, phi}.
        /// </summary>
        public double[] angles
        {
            get
            {
                double[] aangles = new double[2];
                aangles[0] = _theta;
                aangles[1] = _phi;
                return aangles;
            }
        }

        /// <summary>
        /// anchor position (x, y, z) of the protein in the global coordinate system. These values change as the bead rotates.
        /// The protein is anchored on the bead at certain inclination theta and azimuth phi. However the bead may have rotated, so where exactly the anchor is in 3D space depends on both, rotation of the bead and inital anchor
        /// </summary>
        public double[] AnchorPos
        {
            get
            {
                double[] anchorpos = new double[3];
                anchorpos[0] = _R * Math.Sin(_theta + _anchortheta) * Math.Cos(_phi + _anchorphi) + _x;
                anchorpos[1] = _R * Math.Sin(_theta + _anchortheta) * Math.Sin(_phi + _anchorphi) + _y;
                anchorpos[2] = _R * Math.Cos(_theta + _anchortheta) + _z;
                return anchorpos;
            }
        }

    }
}
