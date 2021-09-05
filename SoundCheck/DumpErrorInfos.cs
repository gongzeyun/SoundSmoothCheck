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
    class DumpErrorInfos
    {
        private static Thread mThreadDumpErrorInfo;
        private static Thread mThradPullLogcat;
        private static bool mExit = false;
        private static ConcurrentQueue<ErrorContainer> mErrorQueue = new ConcurrentQueue<ErrorContainer>();


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

            error.dumpPCMData(subPath);
        }
    }
}
