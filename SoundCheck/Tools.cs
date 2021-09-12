using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisioForge.MediaFramework;

namespace SoundCheck
{
    class Tools
    {
        public static int mNormalPCMLengthSaved = 0;
        public static List<byte[]> mNormalPCMDataSaved = new List<byte[]>();

        private static int mFFTSampleBytes = 8192;
        private static double[] mAmpl = new double[mFFTSampleBytes];
        private static double[] mRealIn = new double[mFFTSampleBytes];
        private static double[] mImagIn = new double[mFFTSampleBytes];
        private static double[] mRealOut = new double[mFFTSampleBytes];
        private static double[] mImagOut = new double[mFFTSampleBytes];

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
                //sum_square_ += absValSample * absValSample;
                sum_square_ += absValSample;
            }
            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++");
            //Console.WriteLine("getRMSLevel:" + sum_square_);
            double avr_square= (double)sum_square_ / (length / 2);
            //return Math.Sqrt(avr_square);
            return avr_square;
        }

        public static List<FFTPoint> getFFTPointsFromSavedPcm(int samplerate)
        {
            const int cutoff_freq = 2000;
            const int step_interval = 100;
            const int steps = cutoff_freq / step_interval;
            double[] enery_all_steps = new double[steps];
            double max_energy = -1;
            if (mNormalPCMDataSaved.Count <= 0)
            {
                return null;
            }
            byte[] pcm_data = null;
            lock (mNormalPCMDataSaved)
            {
                pcm_data = new byte[mNormalPCMDataSaved.Last().Length];
            }
            Buffer.BlockCopy(mNormalPCMDataSaved.Last(), 0, pcm_data, 0, pcm_data.Length);
            if (!isPowerOfTwo(pcm_data.Length)) {
                return null;
            }
            List<FFTPoint> result = new List<FFTPoint>();
            for (int i = 0; i < pcm_data.Length; i += 4)
            {
                Int16 valSample = BitConverter.ToInt16(pcm_data, i);
                mRealIn[i / 4] = Convert.ToDouble(valSample);
                mImagIn[i / 4] = 0;
            }
            uint fft_samples = (uint)pcm_data.Length / 4;
            FFT.Compute(fft_samples, mRealIn, mImagIn,mRealOut, mImagOut, true);
            
            FFT.Norm(fft_samples, mRealOut, mImagOut, mAmpl);
            for (int i = 0; i < fft_samples; i++)
            {
                float freq = ((float)samplerate / fft_samples) * i;
                if (freq < cutoff_freq)
                {
                    if (mAmpl[i] > enery_all_steps[(int)freq / step_interval]) {
                        enery_all_steps[(int)freq / step_interval] = mAmpl[i];
                    }
                    if (mAmpl[i] > max_energy)
                    {
                        max_energy = mAmpl[i];
                    }
                }
            }

            for (int i = 0; i < steps; i++)
            {
                result.Add(new FFTPoint((i + 1) * step_interval, enery_all_steps[i] / max_energy));
                Console.WriteLine("getFFTPoints, freq:" + (i + 1) * step_interval + ", eneygy:" + enery_all_steps[i] / max_energy);

            }
            return result;
        }

        public static bool isPowerOfTwo(int num)
        {
            return (num > 0) && (num & (num - 1)) == 0;
        }

        public static void saveNormalPCM(byte[] pcm_data, int length)
        {
            byte[] savePCMData = new byte[length];
            Buffer.BlockCopy(pcm_data, 0, savePCMData, 0, length);
            lock (mNormalPCMDataSaved)
            {
                mNormalPCMDataSaved.Add(savePCMData);
                mNormalPCMLengthSaved += length;
                if (mNormalPCMLengthSaved >= 192 * 2 * 1000)
                {
                    byte[] deleteObject = mNormalPCMDataSaved[0];
                    mNormalPCMLengthSaved -= deleteObject.Length;
                    mNormalPCMDataSaved.RemoveAt(0);
                }
            }
        }
    }
}
