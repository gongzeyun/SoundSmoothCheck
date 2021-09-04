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
            //Console.WriteLine("RMS:" + rms + ", volumeDB:" +  20.0 * Math.Log10(rms));
            return 20.0 * Math.Log10(rms);
        }


        public static Int64 getRecordTime(RecordConfigs config, Int64 sampleSizeSum)
        {
            int bytesOneMS = (config.mSamplerate / 1000) * config.mChannels * (config.mBitFormat >> 3);
            return sampleSizeSum / bytesOneMS;
        }

        private static double getRMSLevel(byte[] pcm_data, int length) 
        {
            Int64 sum_square_ = 0;
            for (int i = 0; i < length; i += 2)
            {
                int valSample = BitConverter.ToInt16(pcm_data, i);
                int absValSample = Math.Abs(valSample);
                sum_square_ += absValSample * absValSample;
            }
            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++");
            //Console.WriteLine("getRMSLevel:" + sum_square_);
            double avr_square= (double)sum_square_ / (length / 2);
            return Math.Sqrt(avr_square);
        }
    }
}
