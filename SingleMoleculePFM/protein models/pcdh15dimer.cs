using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    class pcdh15dimer : protein
    {
        private pcdh15 _firstmonomer;
        private pcdh15 _secondmonomer;

        public pcdh15dimer(pcdh15 monomer1, pcdh15 monomer2)
        {
            _firstmonomer = monomer1;
            _secondmonomer = monomer2;
        }

        public double Protenergy(double z)
        {
            return _firstmonomer.Protenergy(z) + _secondmonomer.Protenergy(z);
        }

        public double PropagateFolding(double force, double dt, double dx)
        {
            return 0;
        }

    }
}
