using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleMoleculePFM.protein_models;

namespace SingleMoleculePFM.assays
{
    class DoubleBeadAssay : assay
    {
        pedestalbead _pb;
        protein _prot;
        particle _probe;

        double _dtheta;
        double _dx;

        /// <summary>
        ///The protein is tethered between a pedestal and a probe particle.
        ///for this assay the origin is assumed to be the original center of the pedestal bead. The anchor point of the protein is on the pedestal (one R in positive x direction from the center of the pedestal)
        ///the protein can freely pivot from 0 to 180 degrees in theta and -90 to 90 degrees in phi; the radial variable of protein extension corresponds to the z variable of the protein.
        /// </summary>
        /// <param name="pb">pedestal bead</param>
        /// <param name="prot">protein</param>
        /// <param name="probe">probe bead</param>
        /// <param name="dx">size of the space increment to use to compute forces</param>
        /// <param name="dtheta">size of the angle increment to use to compute forces</param>
        public DoubleBeadAssay(pedestalbead pb, protein prot, particle probe, double dx, double dtheta)
        {
            _pb = pb;
            _prot = prot;
            _probe = probe;
            _dx = dx;
            _dtheta = dtheta;
        }

        public protein protein
        {
            get
            {
                return _prot;
            }
        }

        /// <summary>
        /// size of the space increment to use to compute forces 
        /// </summary>
        public double dx
        {
            get
            {
                return _dx;
            }
        }

        /// <summary>
        /// size of the angle increment to use to compute forces 
        /// </summary>
        public double dtheta
        {
            get
            {
                return _dtheta;
            }
        }

        /// <summary>
        /// the probe bead
        /// </summary>
        public particle probe {
            get { return _probe; }
        }

        /// <summary>
        /// the free energy of the single molecule assay
        /// </summary>
        public double energy
        {
            get { return GetEnergy(_probe.position[0], _probe.position[1], _probe.position[2], _probe.AnchorPos[0], _probe.AnchorPos[1], _probe.AnchorPos[2]); }
        }

        /// <summary>
        /// forces acting on the linear motion of the probe {Fx, Fy, Fz}
        /// </summary>
        public double[] forcesOnLinMotion
        {
            get
            {
                return GetLocalForces(_probe.position[0], _probe.position[1], _probe.position[2], _probe.AnchorPos[0], _probe.AnchorPos[1], _probe.AnchorPos[2], _dx);
            }
        }

        /// <summary>
        /// forces acting on the rotation of the probe {Ftheta, Fphi}
        /// </summary>
        public double[] forcesOnRotMotion
        {
            get
            {
                return GetLocalForcesRot(_probe.angles[0], _probe.angles[1], _probe.ProteinAnchorAngles[0], _probe.ProteinAnchorAngles[1], _probe.position[0], _probe.position[1], _probe.position[2], _dtheta);
            }
        }

        /// <summary>
        /// free energy of the system (defined by the pedestal bead and the anchored protein) in which the probe is diffusing
        /// </summary>
        /// <param name="probex">x position of the bead, global coordinate</param>
        /// <param name="probey">y position of the bead, global coordinate</param>
        /// <param name="probez">z position of the bead, global coordinate</param>
        /// <param name="protX">x position of the protein anchor on the bead, global coordinates</param>
        /// <param name="protY">y position of the protein anchor on the bead, global coordinates</param>
        /// <param name="protZ">z position of the protein anchor on the bead, global coordinates</param>
        /// <returns>free energy</returns>
        public double GetEnergy(double probex, double probey, double probez, double protX, double protY, double protZ)
        {
            // go to speherical coordinates for the protein anchor on the probe around the anchor on the pedestal.
            //Get the current center position of the pedestal (it fluctuates) and add one Radius to the x coordinate to get the anchor point.
            //Subtract that from the anchor position on the probe to shift the coordinate origin into the anchor point of the pedestal. Then go
            //to spherical coordinates.

            double pb_anchor_x = _pb.X + _pb.R;
            double pb_anchor_y = _pb.Y;
            double pb_anchor_z = _pb.Z;

            //r is the length of the protein tether, i.e. the end to end distance of the protein
            double r = Math.Sqrt(Math.Pow(protX - pb_anchor_x, 2) + Math.Pow(protY - pb_anchor_y, 2) + Math.Pow(protZ - pb_anchor_z, 2));

            //double phi = Math.atan((y - pb_anchor_y) / (x - pb_anchor_x));
            //double theta = Math.acos((z - pb_anchor_z) / r);

            //determine whether the requested coordinates are inside of the pedestal. If they are, return an infinite energy
            // don't forget that the bead has a size.
            double xx = probex - _pb.X;
            double yy = probey - _pb.Y;
            double zz = probey - _pb.Z;
            //rr is the distance between the centers of probe and pedestal.
            double rr = Math.Sqrt(Math.Pow(xx, 2) + Math.Pow(yy, 2) + Math.Pow(zz, 2));
            //double pphi = Math.atan(yy / xx);
            //double ttheta = Math.acos(zz / rr);
            if (rr <= _pb.R + _probe.R)
            {
                //Console.WriteLine("distance from origin: " + rr);
                //Console.WriteLine("position: " + _probe.position[0]);
                //return Double.POSITIVE_INFINITY;
                //Console.WriteLine("IN PEDESTAL, protein length=" + r);
                //return 100 * constants.kB * constants.T * Math.Pow(((_pb.R + _probe.R - rr) / 1e-9), 1);
                return double.PositiveInfinity;
            }
            else //we are not in the pedestal, great. Return the tether energy
            {
                //System.out.println("tether length r: " + r);
                //Console.WriteLine(r);
                return _prot.Protenergy(r);
                
            }
        }


        /// <summary>
        /// this function returns the free energy of the system when passed the position of the particle, and the angular position of the protein anchor (in global angular coorrdinates). 
        /// </summary>
        /// <param name="probex"></param>
        /// <param name="probey"></param>
        /// <param name="probez"></param>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        private double GetEnergySpherical(double probex, double probey, double probez, double theta, double phi)
        {
            double protx = _probe.R * Math.Sin(theta) * Math.Cos(phi) + probex;
            double proty = _probe.R * Math.Sin(theta) * Math.Sin(phi) + probey;
            double protz = _probe.R * Math.Cos(theta) + probez;
            return GetEnergy(probex, probey, probez, protx, proty, protz);
        }

        /// <summary>
        /// calculate the radial forces acting on the probe by taking the gradient of the energy landscape
        /// </summary>
        /// <param name="probex"></param>
        /// <param name="probey"></param>
        /// <param name="probez"></param>
        /// <param name="protX"></param>
        /// <param name="protY"></param>
        /// <param name="protZ"></param>
        /// <param name="dx"></param>
        /// <returns></returns>
        private double[] GetLocalForces(double probex, double probey, double probez, double protX, double protY, double protZ, double dx)
        {
            double[] posforces = new double[3];
            posforces[0] = -(GetEnergy(probex + dx, probey, probez, protX + dx, protY, protZ) - GetEnergy(probex, probey, probez, protX, protY, protZ)) / dx;
            posforces[1] = -(GetEnergy(probex, probey + dx, probez, protX, protY + dx, protZ) - GetEnergy(probex, probey, probez, protX, protY, protZ)) / dx;
            posforces[2] = -(GetEnergy(probex, probey, probez + dx, protX, protY, protZ + dx) - GetEnergy(probex, probey, probez, protX, protY, protZ)) / dx;
            //Console.WriteLine(posforces[0]);
            if (Math.Abs(posforces[0]) > 1e-10 || Math.Abs(posforces[1]) > 1e-10 || Math.Abs(posforces[2]) > 1e-10)
            {
                //Console.WriteLine("stop");
            }

            int i;
            for (i = 0; i < 3; i++)
            {
                if (double.IsNaN(posforces[i]) || double.IsInfinity(posforces[i]))
                {
                    //Console.WriteLine(posforces[0]);
                    posforces[i] = 0;
                }
            }
            return posforces;
        }

        /// <summary>
        /// calculate the tangential forces acting on the probe by taking the gradient of the energy landscape for rotations.
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        /// <param name="anchortheta"></param>
        /// <param name="anchorphi"></param>
        /// <param name="probex"></param>
        /// <param name="probey"></param>
        /// <param name="probez"></param>
        /// <param name="dtheta"></param>
        /// <returns></returns>
        private double[] GetLocalForcesRot(double theta, double phi, double anchortheta, double anchorphi, double probex, double probey, double probez, double dtheta)
        {
            //x, y, z are the anchor position of the protein on the probe
            //XX, YY, ZZ are the center position of the diffusing probe
            double[] angleforces = new double[2];
            double dx_dth = _probe.R * Math.Cos(theta) * Math.Cos(phi) * dtheta;
            double dy_dth = _probe.R * Math.Cos(theta) * Math.Sin(phi) * dtheta;
            double dz_dth = -_probe.R * Math.Sin(theta) * dtheta;
            double dx_dphi = -_probe.R * Math.Sin(theta) * Math.Sin(phi) * dtheta;
            double dy_dphi = _probe.R * Math.Sin(theta) * Math.Cos(phi) * dtheta;
            double dz_dphi = 0;

            angleforces[0] = -(GetEnergySpherical(probex, probey, probez, anchortheta + theta + dtheta, anchorphi + phi) - GetEnergySpherical(probex, probey, probez, anchortheta + theta, anchorphi + phi)) / (_probe.R * dtheta);

            angleforces[1] = -(GetEnergySpherical(probex, probey, probez, anchortheta + theta, anchorphi + phi + dtheta) - GetEnergySpherical(probex, probey, probez, anchortheta + theta, anchorphi + phi)) / (_probe.R * dtheta);

            int i;

            for (i = 0; i < 2; i++)
            {
                if (double.IsNaN(angleforces[i]) || double.IsInfinity(angleforces[i]))
                {
                    angleforces[i] = 0;
                }
            }

            return angleforces;
        }

        public void AdvanceEnergy(double dt) { } // not yet implemented.

    }
}
