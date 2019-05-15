using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM.protein_models
{
    /// <summary>
    /// Defines a protocadherin molecule.
    /// In this implementation, the protein only has two extensions, l1 and l2, between which it can switch by folding/unfolding
    /// Not yet implemented: calcium dependent folding
    /// </summary>
    class pcdh15 : protein
    {
        /// <summary>
        /// current length of the protein </summary>
        private double _L;
        /// <summary>
        /// possible length_1 of the protein </summary>
        private double _l1;
        /// <summary>
        /// possible length_2 of the protein </summary>
        private double _l2;
        /// <summary>
        /// persistence length </summary>
        private double _lp;

        /// <summary>
        /// internal coordinate of the unfolded minimum
        /// </summary>
        private double _minunfolded;

        /// <summary>
        /// internal coordinate of folded minimum
        /// </summary>
        private double _minfolded;

        /// <summary>
        /// internal spring constant of folded state
        /// </summary>
        private double _kfolded;

        /// <summary>
        /// internal spring constant of unfolded state
        /// </summary>
        private double _kunfolded;

        private double _discloc;

        /// <summary>
        /// internal coordinate that we diffuse around to figure out whether we should unfold
        /// </summary>
        private double _int_z;

        private Random _rng;

        /// <summary>
        /// models pcdh15
        /// </summary>
        /// <param name="l1">possible length_1 of the protein</param>
        /// <param name="l2">possible length_2 of the protein</param>
        /// <param name="lp">persistence length</param>
        /// <param name="minfolded">internal coordinate of folded minimum</param>
        /// <param name="minunfolded">internal coordinate of the unfolded minimum</param>
        /// <param name="kfolded">internal spring constant of folded state</param>
        /// <param name="kunfolded">internal spring constant of unfolded state</param>

        public pcdh15(double l1, double l2, double lp, double minfolded, double minunfolded, double kfolded, double kunfolded, double discloc)
        {
            _L = l1;
            _l1 = l1;
            _l2 = l2;
            _lp = lp;
            _minfolded = minfolded;
            _minunfolded = minunfolded;
            _kfolded = kfolded;
            _kunfolded = kunfolded;
            _discloc = discloc;
            _rng = new Random();
            _int_z = minfolded;
        }

        /// <summary>
        /// Calculates the free energy if the protein is stretched to an end-to-end distance of z
        /// </summary>
        /// <param name="z">End-to-end distance of the protein</param>
        /// <returns>Free energt of the protein at end-to-end distance z</returns>
        public double Protenergy(double z)
        {
            //WLC energy from integrating the F-x relation from wikipedia
            double energy = constants.kB * constants.T / (4 * _lp) * (Math.Pow(_L, 2) / (_L - z) - z + 2 * Math.Pow(z, 2) / _L);
            if(Double.IsInfinity(energy))
            {
                Console.WriteLine("protein energy infinity");
            }
            if (Double.IsNaN(energy))
            {
                Console.WriteLine("protein energy NaN");
            }
            return energy;
        }

        /// <summary>
        /// propagates the internal state of the pcdh15 protein by testing whether we should un/refold
        /// </summary>
        public double PropagateFolding(double force, double dt, double dx)
        {
            double intdrag = 6 * Math.PI * 8.9e-4 * 500e-9 *10000;//made up another number
            double intdiffconst = constants.kB*constants.T/intdrag; //hard coded into here. Who knows how fast it should be.
            
            double totalforce = -(internalEnergy(_int_z + dx) - internalEnergy(_int_z)) / dx + force;
            _int_z += Math.Sqrt(2 * intdiffconst * dt) * _rng.NextGaussian() + dt * totalforce/ intdrag;

            if(_int_z<_discloc)
            {
                _L = _l1;
                Console.WriteLine("l1   " + _int_z);
            }
            else
            {
                _L = _l2;
                Console.WriteLine("l2   " + _int_z);
            }

            return 0.0;
        }

        private double internalEnergy(double z)
        {
            if(z<=_discloc)
            {
                return 1 / 2 * _kfolded * Math.Pow((z - _minfolded), 2);
            }
            else
            {
                return 1 / 2 * _kunfolded * Math.Pow((z - _minunfolded), 2); //+ 0.5 * (_kfolded * Math.Pow((_discloc - _minfolded), 2)-_kunfolded*Math.Pow((_discloc-_minunfolded),2));
            }
        }
    }
}
