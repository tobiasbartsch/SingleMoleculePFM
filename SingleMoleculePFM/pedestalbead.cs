using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    class pedestalbead
    {
        private double _xmean;
        private double _ymean;
        private double _zmean;
        private double _R;
        private double _sdev;
        private Random rng;

        public pedestalbead(double R, double sdev, double xmean, double ymean, double zmean)
        {
            _R = R;
            _sdev = sdev;
            _xmean = xmean;
            _ymean = ymean;
            _zmean = zmean;
            rng = new Random();
        }

        /// <summary>
        /// radius of the pedestal bead
        /// </summary>
        public double R
        {
            get
            {
                return _R;
            }
        }
                
        /// <summary>
        /// pedestal x position
        /// </summary>
        public double X
        {
            get
            {
                return (_xmean + _sdev * rng.NextGaussian());
            }
        }

        /// <summary>
        /// pedestal y position
        /// </summary>
        public double Y
        {
            get
            {
                return (_ymean + _sdev * rng.NextGaussian());
            }
        }

        /// <summary>
        /// pedestal z position
        /// </summary>
        public double Z
        {
            get
            {
                return (_zmean + _sdev * rng.NextGaussian());
            }
        }

        public void AdvancePedestalPosition(double dt) { } //not yet implemented. Right now we are just drawing a random position, do not have dynamics yet
    }
}
