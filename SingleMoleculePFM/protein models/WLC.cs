using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM.protein_models
{

    class WLC : protein
    {
        private double _lp;
        private double _lc;

        public WLC(double lp, double lc)
        {
            _lp = lp; //persistence length
            _lc = lc; //countour length
        }

        public double Protenergy(double z)
        {
            double energy = 0;

            energy = constants.kB * constants.T * (-_lc * z + 2 * Math.Pow(z, 2) - Math.Pow(z, 3) / (z - _lc));
            energy = energy / (4 * _lc * _lp);
            return energy;
            //return 0;
        }

    }
}
