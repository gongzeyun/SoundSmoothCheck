using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    class Tools
    {
        public static void dumpRecordPCM(String filePath, byte[] pcm, int size)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create | FileMode.Append));
            writer.Write(pcm, 0, size);
            writer.Flush();
            writer.Close();
        }

        public static double getVolumeDB(byte[] pcm_data, int length)
        {
            double rms = getRMSLevel(pcm_data, length);
            return 20.0 * Math.Log10(rms);
        }


        public static Int64 getRecordTime(RecordConfigs config, Int64 sampleSizeSum)
        {
            int bytesOneMS = (config.mSamplerate / 1000) * config.mChannels * (config.mBitFormat >> 3);
            return sampleSizeSum / bytesOneMS;
        }

        private static double getRMSLevel(byte[] pcm_data, int length) 
        {
            UInt64 sum_square_ = 0;
            ushort[] short_pcm_data = new ushort[length / 2];
            Buffer.BlockCopy(pcm_data, 0, short_pcm_data, 0, length);
            for (int i = 0; i < short_pcm_data.Length; ++i) {
                sum_square_ = sum_square_ +  (UInt64)short_pcm_data[i] * short_pcm_data[i];
            }
            return Math.Sqrt(sum_square_ / ((UInt64)short_pcm_data.Length));
        }
    }
}
