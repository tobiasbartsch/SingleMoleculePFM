using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM
{
    /// <summary>
    /// Defines a maximum length sequence.
    /// </summary>
    class msequence
    {
        #region fields
        int[] _msequence;
        int _length;
        double _dt_mseq;
        int _current_position;
        double _fraction_position;
        #endregion

        /// <summary>
        /// Create a new msequence object.
        /// </summary>
        /// <param name="filename">String to the path from which to load the msequence (one column of 1s and 0s).</param>
        /// <param name="dt_mseq">Time in seconds between successive entries of the sequence.</param>
        public msequence(string filename, double dt_mseq)
        {
            _msequence = utils.readmsequence(filename);
            _dt_mseq = dt_mseq;
            _length = _msequence.Length;
            _fraction_position = 0; // current position between steps in the msequence;
            _current_position = 0; //current position in the msequence.
        }

        #region properties
        /// <summary>
        /// The current value of the msequence.
        /// </summary>
        public int value
        {
            get
            {
                return _msequence[_current_position];
            }
        }

        /// <summary>
        /// The time between successive msequence elements in seconds.
        /// </summary>
        public double dt_mseq
        {
            get
            {
                return _dt_mseq;
            }
        }



        /// <summary>
        /// The current fractional position between steps in the msequence.
        /// </summary>
        /// <value>The fraction position.</value>
        public double fractionPosition
        {
            get 
            {
                return _fraction_position;
            }
        }
        
        #endregion

        #region methods
        /// <summary>
        /// Advance the msequence by one step. If the msequence is at the end, reset it to zero
        /// </summary>
        private void AdvanceMsequence()
        {
            if(_current_position < _length)
            {
                _current_position++;
                _fraction_position = 0; // reset the fraction position to zero
            }
            else
            {
                _current_position = 0;
                _fraction_position = 0;
            }
        }

        /// <summary>
        /// Propagate msequence by <paramref name="dt"/>.
        /// </summary>
        /// <param name="dt">Advance msequence by dt seconds.</param>
        public void PropagateMsequenceFraction(double dt)
        {
            _fraction_position += dt/_dt_mseq;

            if (_fraction_position >= 1)
            {
                AdvanceMsequence();
            }
        }

        /// <summary>
        /// Reset the position in the msequence to 0
        /// </summary>
        public void ResetMsequence()
        {

            _current_position = 0;
            _fraction_position = 0;
           
        }



        /// <summary>
        /// Replace the current sequence with a new sequence.
        /// </summary>
        /// <param name="filename">String to the path from which we will load the new sequence.</param>
        /// <param name="dt">Time in seconds between successive entries of the sequence.</param>
        public void reload_sequence(string filename, double dt)
        {
            _msequence = utils.readmsequence(filename);
            _dt_mseq = dt;
            _length = _msequence.Length;
        }

        #endregion

    }
}
