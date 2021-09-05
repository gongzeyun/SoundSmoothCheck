using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCheck
{
    static class DumpErrorInfos
    {
        private static Thread mThreadDumpErrorInfo;
        private static bool mExit = false;
        private static ConcurrentQueue<ErrorContainer> mErrorQueue = new ConcurrentQueue<ErrorContainer>();

        private static List<KeyValuePair<String, String>> mErrorsHistory = new List<KeyValuePair<String, String>>();
        public static void startErrorDumpTask()
        {
            mThreadDumpErrorInfo = new Thread(dumpErrorIntoThread);
            mThreadDumpErrorInfo.Start();
            mExit = false;
        }

        public static void exitErrorDumpTask()
        {
            mExit = true;
        }

        public static void dumpErrorIntoThread()
        {
            ErrorContainer errorDump = new ErrorContainer();
            while (!mExit)
            {
                if (!mErrorQueue.IsEmpty)
                {
                    if (mErrorQueue.TryDequeue(out errorDump))
                    {
                        dumpErrorInfo(errorDump);
                    }
                }
                Thread.Sleep(100);
            }
        }

        public static void enterDumpQueue(ErrorContainer error)
        {
            mErrorQueue.Enqueue(error);
        }

        private static void dumpErrorInfo(ErrorContainer error)
        {
            Console.WriteLine("dumpErrorInfo, error_time:" + error.getErrorOccuredTime());
            string currPath = Application.StartupPath;
            string subPath = currPath + "/" + error.getErrorOccuredTime();
            if (false == System.IO.Directory.Exists(subPath))
            {
                System.IO.Directory.CreateDirectory(subPath);
            }
            error.setReportPath(subPath);
            error.dumpPCMData(subPath);

            mErrorsHistory.Add(new KeyValuePair<String,String>(error.getErrorOccuredTime(), subPath));
        }

        public static String getErrorReportPathByErrorTime(String errorOccurTime)
        {
            for (int i = 0; i < mErrorsHistory.Count; i++)
            {
                KeyValuePair<String, String> errorHistoryElement = mErrorsHistory[i];
                if (errorHistoryElement.Key.Equals(errorOccurTime))
                {
                    return errorHistoryElement.Value;
                }
            }

            return "";
        }
    }
}
