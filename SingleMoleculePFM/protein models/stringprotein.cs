using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    class stringprotein : protein
    {
        private double _length;
        public stringprotein(double length)
        {
            _length = length;
        }

        public double Protenergy(double z)
        {
            //System.out.println("length: " + z);
            if (z <= _length)
            {
                return 0;
            }
            else
            {
                return 4 * constants.kB * constants.T * (z - _length) / 10e-9;
            }
        }

        public double PropagateFolding(double force, double dt, double dx)
        {
            return 0;
        }
    }
}
