using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    public class TimeAndVolumeDBPoint
    {
        public Int64 mTime;
        public double mVolumeDB;

        public TimeAndVolumeDBPoint(Int64 time, double volumeDB)
        {
            mTime = time;
            mVolumeDB = volumeDB;
        }
    }
}
