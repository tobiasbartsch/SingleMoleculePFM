using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM.protein_models
{
    /// <summary>
    /// Protein model of an (almost) inextensible stick. Useful to test what kind of data we would expect if the protein were very stiff.
    /// </summary>
    /// <remarks>
    /// The protein has to have some way of reacting to external forces, so we will consider it to have a very stiff spring constant.
    /// At a maximal force of 100 pN we want the protein to stretch by 1 nm -> k_protein = 100 pN/nm = 100 mN/m
    /// </remarks>
    class StickProtein : protein
    {
        private double _length;
        private double _k_prot = 0.05; //0.05 mN/m

        public StickProtein(double length)
        {
            _length = length; //length
        }

        public double Protenergy(double z)
        {
            double energy = 0;

            energy = 0.5 * _k_prot * Math.Pow(Math.Abs(z - _length), 2);
            return energy;
        }

    }
}
