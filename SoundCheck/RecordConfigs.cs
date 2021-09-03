using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    class RecordConfigs
    {
        public int mSamplerate;
        public int mChannels;
        public int mBitFormat;
        public String mReadableName;
        public int mPeriodSize;
        public RecordConfigs(int sample, int channels, int format, String name, int period_size)
        {
            mSamplerate = sample;
            mChannels = channels;
            mBitFormat = format;
            mReadableName = name;
            mPeriodSize = period_size;
        }
        public String MyToString()
        {
            return "samplerate:" + mSamplerate + ", channels:" + mChannels + ", format:" + mBitFormat + ", period_size:" + mPeriodSize;
        }
    }
}
