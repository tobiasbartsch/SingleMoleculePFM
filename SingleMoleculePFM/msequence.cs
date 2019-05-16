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

        #endregion

        #region methods
        /// <summary>
        /// Advance the msequence by one step.
        /// </summary>
        public void AdvanceMsequence()
        {
            if(_current_position < _length)
            {
                _current_position++;
            }
        }


        /// <summary>
        /// Replace the current sequence with a new sequence.
        /// </summary>
        /// <param name="filename">String to the path from which we will load the new sequence.</param>
        /// <param name="dt">Time in seconds between successive entries of the sequence.</param>
        public void reload_sequence(string filename, double dt)
        {
            _msequence = utils.readmsequence(filename);
            _dt = dt;
            _length = _msequence.Length;
        }

        #endregion

    }
}
