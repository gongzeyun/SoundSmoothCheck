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

        public RecordConfigs(int sample, int channels, int format, String name)
        {
            mSamplerate = sample;
            mChannels = channels;
            mBitFormat = format;
            mReadableName = name;
        }
        public String MyToString()
        {
            return "samplerate:" + mSamplerate + ", channels:" + mChannels + ", format:" + mBitFormat;
        }
    }
}
