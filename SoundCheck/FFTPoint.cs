using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    class FFTPoint
    {
        public double mFreq;
        public double mNormValue;

        public FFTPoint(double freq, double normValue)
        {
            mFreq = freq;
            mNormValue = normValue;
        }
    }
}
