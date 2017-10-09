using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    /// <summary>
    /// Defines a protocadherin molecule whose energy landscape depends on the amount of calcium bound to it.
    /// In this implementation, the protein only has two energy minima
    /// </summary>
    class pcdh15 : protein
    {
        /// <summary>
        /// spring constant of first energy minimum </summary>
        private double _k1;
        /// <summary>
        /// spring constant of the second energt minimum </summary>
        private double _k2;
        /// <summary>
        /// location of the first minimum along the reaction coordinate z </summary>
        private double _min1;
        /// <summary>
        /// location of the second minimum along the reaction coordinate z </summary>
        private double _min2;
        /// <summary>
        /// location of the energy landscape discontinuity. To the left of this location the protein feels minimum 1, to right it feels minimum 2 </summary>
        private double _discloc; //location of energy landscape discontinuity in m

        /// <summary>
        /// models pcdh15
        /// </summary>
        /// <param name="min1">position of the first energy well in m</param>
        /// <param name="k1">spring constant of the first energy well</param>
        /// <param name="min2">position of the second energy well in m</param>
        /// <param name="k2">spring constant of the second energy well</param>
        /// <param name="discloc">position of the discontinuity</param>
        public pcdh15(double min1, double k1, double min2, double k2, double discloc)
        {
            _k1 = k1;
            _k2 = k2;
            _min1 = min1;
            _min2 = min2;
            _discloc = discloc;
        }

        /// <summary>
        /// Calculates the free energy if the protein is stretched to an end-to-end distance of z
        /// </summary>
        /// <param name="z">End-to-end distance of the protein</param>
        /// <returns>Free energt of the protein at end-to-end distance z</returns>
        public double Protenergy(double z)
        {
            if (z <= _discloc)
            {
                return 0.5 * _k1 * Math.Pow(z - _min1, 2);
            }
            else
            {
                return 0.5 * _k2 * Math.Pow(z - _min2, 2); //+ 0.5 * (_k1 * Math.Pow(_discloc - _min1, 2) - _k2 * Math.Pow(_discloc - _min2, 2));
            }
        }
    }
}
