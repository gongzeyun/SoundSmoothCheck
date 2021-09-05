using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    class ErrorContainer
    {
        private DateTime mErrorOccuredTime;
        private List<byte[]> mSavedErrorPCMData = new List<byte[]>();
        private int mSavedPCMLength = 0;

        private static List<byte[]> mSavedNormalPCMData = new List<byte[]>();
        private static int mSavedNormalPCMLength = 0;

        public static int ERROR_IS_MAKEING = 0;
        public static int ERROR_ENTERED_QUEUE = 1;
        private int mState;

        public ErrorContainer(DateTime errorTime)
        {
            Console.WriteLine("ErrorContainer construct");
            mState = ERROR_IS_MAKEING;
            mSavedPCMLength = 0;
            mErrorOccuredTime = errorTime;
            for (int i = 0; i < mSavedNormalPCMData.Count; i++)
                saveErrorPCMData(mSavedNormalPCMData[i], mSavedNormalPCMData[i].Length);
        }
        public ErrorContainer()
        {
        }
        public String getErrorOccuredTime()
        {
            return mErrorOccuredTime.ToString("yyyyMMddHHmmss");
        }

        public void saveErrorPCMData(byte[] pcm_data, int length)
        {
            Console.WriteLine("saveErrorPCMData, length:" + length);
            byte[] savePCMData = new byte[length];
            Buffer.BlockCopy(pcm_data, 0, savePCMData, 0, length);
            mSavedErrorPCMData.Add(savePCMData);
            mSavedPCMLength += length;
            if (mSavedPCMLength >= 192 * 4 * 1000)
            {
                mState = ERROR_ENTERED_QUEUE;
                DumpErrorInfos.enterDumpQueue(this);
            }
        }

        public static void saveNormalPCM(byte[] pcm_data,int length)
        {
            byte[] savePCMData = new byte[length];
            Buffer.BlockCopy(pcm_data, 0, savePCMData, 0, length);
            mSavedNormalPCMData.Add(savePCMData);
            mSavedNormalPCMLength += length;
            if (mSavedNormalPCMLength >= 192 * 2 * 1000)
            {
                byte[] deleteObject = mSavedNormalPCMData[0];
                mSavedNormalPCMLength -= deleteObject.Length;
                mSavedNormalPCMData.RemoveAt(0);
            }
            //Console.WriteLine("saveNormalPCM, length:" + mSavedNormalPCMLength);
        }
        public int getState()
        {
            return mState;
        }

        public void dumpPCMData(String dir)
        {
            for (int i = 0; i < mSavedErrorPCMData.Count; i++)
            {
                Tools.dumpRecordPCM(dir + "/dump.pcm", mSavedErrorPCMData[i], mSavedErrorPCMData[i].Length);
            }
        }
    }
}
