using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    /// <summary>
    /// simulates a protein that unfolds with a 3nm step at fairly high forces (around 30 pN).
    /// </summary>
    class One3nmStep : protein
    {
        #region fields
        private double _posfirstmin;
        private double _k = 14e-3;
        private double _distance = 3e-9;
        private double _unfoldingForce = 30e-12;
        #endregion fields


        public One3nmStep(double posfirstmin)
        {
            _posfirstmin = posfirstmin;
        }

        public double Protenergy(double z)
        {
            double energy = 0;
            if(z < _posfirstmin+ _distance/2)
            {
                energy = 0.5 * _k * Math.Pow((z-_posfirstmin), 2);
            }
            else
            {
                energy = 0.5 * _k * Math.Pow((z - (_posfirstmin+ _distance)), 2);
            }
            return energy + _unfoldingForce * z;
            //return 0;
        }
    }
}
