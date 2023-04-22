using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SoundCheck
{
    static class DumpErrorInfos
    {
        private static Thread mThreadDumpErrorInfo;
        private static bool mExit = false;
        private static ConcurrentQueue<ErrorContainer> mErrorQueue = new ConcurrentQueue<ErrorContainer>();

        private static List<KeyValuePair<String, ErrorHistory>> mErrorsHistory = new List<KeyValuePair<String, ErrorHistory>>();
        public class ErrorHistory
        {
            public String mErrorReportPath;
            public Int64 mTimeRecordTimeMS;

            public ErrorHistory(String path, Int64 timeRecordTimeMs)
            {
                mErrorReportPath = path;
                mTimeRecordTimeMS = timeRecordTimeMs;
            }
        }
        public static void startErrorDumpTask()
        {
            mThreadDumpErrorInfo = new Thread(saveErrorIntoThread);
            mThreadDumpErrorInfo.Start();

            mExit = false;
        }

        public static void exitErrorDumpTask()
        {
            mExit = true;
        }

        public static void saveErrorIntoThread()
        {
            ErrorContainer errorDump;//= new ErrorContainer();
            while (!mExit)
            {
                if (!mErrorQueue.IsEmpty)
                {
                    if (mErrorQueue.TryDequeue(out errorDump))
                    {
                        saveErrorInfo(errorDump);
                        pullLogcat(errorDump);
                    }
                }
                Thread.Sleep(100);
            }
        }

        public static void enterDumpQueue(ErrorContainer error)
        {
            mErrorQueue.Enqueue(error);
        }

        private static void saveErrorInfo(ErrorContainer error)
        {
            Console.WriteLine("dumpErrorInfo, error_time:" + error.getErrorOccuredTime());
            string currPath = Application.StartupPath;
            string subPath = currPath + "\\" + error.getErrorOccuredTime();
            if (false == System.IO.Directory.Exists(subPath))
            {
                System.IO.Directory.CreateDirectory(subPath);
            }
            error.setReportPath(subPath);
            error.dumpPCMData(subPath);
            mErrorsHistory.Add(new KeyValuePair<String, ErrorHistory>(error.getErrorOccuredTime(), new ErrorHistory(error.getReportPath(), error.getRecordTimeMS())));
        }

        public static String getErrorReportPathByErrorRealTime(String errorOccurTime)
        {
            for (int i = 0; i < mErrorsHistory.Count; i++)
            {
                KeyValuePair<String, ErrorHistory> errorHistoryElement = mErrorsHistory[i];
                if (errorHistoryElement.Key.Equals(errorOccurTime))
                {
                    return errorHistoryElement.Value.mErrorReportPath;
                }
            }

            return "";
        }

        public static Int64 getRecordTimeByErrorRealTime(String errorOccurTime)
        {
            for (int i = 0; i < mErrorsHistory.Count; i++)
            {
                KeyValuePair<String, ErrorHistory> errorHistoryElement = mErrorsHistory[i];
                if (errorHistoryElement.Key.Equals(errorOccurTime))
                {
                    return errorHistoryElement.Value.mTimeRecordTimeMS;
                }
            }

            return -1;
        }

        private static void pullLogcat(ErrorContainer error)
        {
            // new process对象
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            // 设置属性
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow =true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            String command = string.Empty;
            command += "adb root&adb pull /data/misc/logd/ " + error.getReportPath() + "\\logcat\\";
            Console.WriteLine("command:" + command);

            // 开启process线程
            p.Start();
            p.StandardInput.WriteLine(command);

           

            StreamReader readerout = p.StandardOutput;
            string line = string.Empty;
            line = readerout.ReadLine();
            Console.WriteLine(line);
            p.WaitForExit(5000);
            p.Close();


            Process pRunBAT = new Process();
            ProcessStartInfo pi = new ProcessStartInfo("ops.bat", error.getReportPath());//第二个参数为传入的参数，string类型以空格分隔各个参数
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;
            pRunBAT.StartInfo = pi;
            pRunBAT.Start();
            pRunBAT.WaitForExit();
            pRunBAT.Close();
        }
    }
}
