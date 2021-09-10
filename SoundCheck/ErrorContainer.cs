using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    public class ErrorContainer
    {
        private DateTime mErrorOccuredTime;
        private List<byte[]> mSavedErrorPCMData = new List<byte[]>();
        private int mSavedPCMLength = 0;

        private static List<byte[]> mSavedNormalPCMData = new List<byte[]>();
        private static int mSavedNormalPCMLength = 0;

        public static int ERROR_STATE_MAKEING = 0;
        public static int ERROR_STATE_QUEUEED = 1;
        public static int ERROR_STATE_SAVED = 2;

        private int mState = ERROR_STATE_MAKEING;
        private String mReportPath;
        private Int64 mTimeMsRecord;
        public ErrorContainer(DateTime errorTime, Int64 timeMsRecord)
        {
            Console.WriteLine("ErrorContainer construct, occuredTime:" + errorTime.ToString("yyyyMMddHHmmss") + ", timMSRecord:" + timeMsRecord);
            mState = ERROR_STATE_MAKEING;
            mSavedPCMLength = 0;
            mErrorOccuredTime = errorTime;
            mTimeMsRecord = timeMsRecord;
            for (int i = 0; i < mSavedNormalPCMData.Count; i++)
                saveErrorPCMData(mSavedNormalPCMData[i], mSavedNormalPCMData[i].Length);
        }
        public Int64 getRecordTimeMS() {
            return mTimeMsRecord;
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
            Console.WriteLine("saveErrorPCMData, mSavedPCMLength:" + mSavedPCMLength);
            byte[] savePCMData = new byte[length];
            Buffer.BlockCopy(pcm_data, 0, savePCMData, 0, length);
            mSavedErrorPCMData.Add(savePCMData);
            mSavedPCMLength += length;
            if (mSavedPCMLength >= 192 * 4 * 1000 && mState == ERROR_STATE_MAKEING)
            {
                DumpErrorInfos.enterDumpQueue(this);
                mState = ERROR_STATE_QUEUEED;
            }
        }

        public static void normalPCMSaved(byte[] pcm_data,int length)
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
                mState = ERROR_STATE_SAVED;

            }
        }

        public void setReportPath(String reportPath)
        {
            mReportPath = reportPath;
        }

        public String getReportPath()
        {
            return mReportPath;
        }
    }
}
