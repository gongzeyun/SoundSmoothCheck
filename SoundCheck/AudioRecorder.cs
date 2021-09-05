using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Microsoft;
using NAudio.CoreAudioApi;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Concurrent;

namespace SoundCheck
{
    class AudioRecorder
    {
        List<KeyValuePair<int, RecordConfigs>> mRecordConfigs = new List<KeyValuePair<int, RecordConfigs>>();
        VolumeDBUpdateListener mVolumeDBUpdateListener;
        ErrorReportListener mErrorReportListener;
        private int mSelectedDevice = 0;
        private int mSelectedConfig = 0;
        private static Int64 mRecordSampleSizeSum = 0;
        private const int mRecordPeriodSize = 4096;
        private static byte[] mRecordPCMData = new byte[mRecordPeriodSize];
        private CallbackDelegate mCallBackFunction;

        public static int RECORD_STATE_OPENED = 0;
        public static int RECORD_STATE_CAPTURING = 1;
        public static int RECORD_STATE_CLOSED = 2;

        private int mRecordState = RECORD_STATE_CLOSED;

        private int mMinAlarmValue;
        private int mMaxAlarmValue;

        private ErrorContainer mErrorContainer = null;

        public int getRecordState()
        {
            return mRecordState;
        }
        public static int getDeviceCount()
        {
            return get_device_count_fromdll();
        }

        public AudioRecorder()
        {
            mCallBackFunction = CallBackFromCLanuage;
            register_C_msg_callback_fromdll(mCallBackFunction);
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000100, new RecordConfigs(44100, 1, 8, "44100, Mono, 8bit", mRecordPeriodSize)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000200, new RecordConfigs(44100, 2, 8, "44100, Stereo, 8bit", mRecordPeriodSize)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000400, new RecordConfigs(44100, 1, 16, "44100, Mono, 16bit", mRecordPeriodSize)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000800, new RecordConfigs(44100, 2, 16, "44100, Stereo, 16bit", mRecordPeriodSize)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00001000, new RecordConfigs(48000, 1, 8, "48000, Mono, 8bit", mRecordPeriodSize)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00002000, new RecordConfigs(48000, 2, 8, "48000, Stereo, 8bit", mRecordPeriodSize)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00004000, new RecordConfigs(48000, 1, 16, "48000, Mono, 16bit", mRecordPeriodSize)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00008000, new RecordConfigs(48000, 2, 16, "48000, Stereo, 16bit", mRecordPeriodSize)));
        }
        public String getDeviceName(int deviceIndex)
        {
            Console.WriteLine("getDeviceName, deviceIndex:" + deviceIndex);
            IntPtr ptrName = get_device_name_fromdll(deviceIndex);
            return Marshal.PtrToStringUni(ptrName);
        }

        public List<String> getDeviceConfigs(int deviceIndex)
        {
            List<String> deviceConfigs = new List<string>();
            

            Console.WriteLine("getDeviceConfigs, deviceIndex:" + deviceIndex);
            int valConfig = get_configs_device_support_fromdll(deviceIndex);                               
            for (int i = 0; i < mRecordConfigs.Count; i++)
            {
                KeyValuePair<int, RecordConfigs> tmpKeyValue = mRecordConfigs[i];
                if (0 != (valConfig & tmpKeyValue.Key))
                {
                    deviceConfigs.Add(tmpKeyValue.Value.mReadableName);
                }
            }
            if (0 == deviceConfigs.Count)
            {
                deviceConfigs.Add("44100, Mono, 8bit"); //this is default value
            }
            return deviceConfigs;
        }


        public void startRecord()
        {
            RecordConfigs recordConfigSelected = mRecordConfigs[mSelectedConfig].Value;
            Console.WriteLine("AudioRecord###startRecord, select device:" + mSelectedDevice + ", record config:" + recordConfigSelected.MyToString());
            start_audio_record_fromdll(mSelectedDevice, recordConfigSelected.mSamplerate, recordConfigSelected.mChannels, recordConfigSelected.mBitFormat,
                mRecordPeriodSize);
        }

        public void processRecordPCMData()
        {
            double volumeDB = Tools.getVolumeDB(mRecordPCMData, mRecordPCMData.Length);
            Int64 timeMS = Tools.getRecordTime(mRecordConfigs[mSelectedConfig].Value, mRecordSampleSizeSum);        
            if (mVolumeDBUpdateListener != null)
            {
                mVolumeDBUpdateListener.onVolumeDBUpdate(new TimeAndVolumeDBPoint(timeMS, volumeDB));
            }

            if (mErrorContainer != null)
            {
                if (mErrorContainer.getState() == ErrorContainer.ERROR_IS_MAKEING)
                {
                    mErrorContainer.saveErrorPCMData(mRecordPCMData, mRecordPCMData.Length);
                }
                else
                {
                    //notify UI thread to display Error link label
                    if(mErrorReportListener != null)
                    {
                        mErrorReportListener.onErrorReport(mErrorContainer);
                    }
                    mErrorContainer = null;
                }
                return;
            }
            //Console.WriteLine("volumeDB:" + volumeDB + ", min alarm:" + mMinAlarmValue + ", max alarm:" + mMaxAlarmValue);
            if ((volumeDB < mMinAlarmValue || volumeDB > mMaxAlarmValue) && mErrorContainer == null)
            {
                mErrorContainer = new ErrorContainer(DateTime.Now);
            }

            return;
        }
        

        public void stopRecord()
        {
            stop_audio_record_fromdll();
        }

        public void selectDevice(int index)
        {
            mSelectedDevice = index;
        }

        public void selectConfig(int index)
        {
            mSelectedConfig = index;
        }

        [DllImport("ssc_core.dll")]
        public extern static int get_device_count_fromdll();

        [DllImport("ssc_core.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr get_device_name_fromdll(int deviceIndex);

        [DllImport("ssc_core.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int get_configs_device_support_fromdll(int deviceIndex);

        [DllImport("ssc_core.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int start_audio_record_fromdll(int index, int samplerate, int channels, int format, int period_size);

        [DllImport("ssc_core.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int stop_audio_record_fromdll();

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void CallbackDelegate(int cmd, IntPtr data, int para_length);

        private void CallBackFromCLanuage(int cmd, IntPtr data, int para_length)
        {
            switch (cmd)
            {
                case MsgCLanguage.CMD_RECORD_STARTED:
                    Console.WriteLine("CMD_RECORD_STARTED received");
                    mRecordSampleSizeSum = 0;
                    mRecordState = RECORD_STATE_OPENED;
                    break;
                case MsgCLanguage.CMD_RECORD_DATA_AVALIABLE:
                    Marshal.Copy(data, mRecordPCMData, 0, para_length);
                    mRecordSampleSizeSum += para_length;
                    processRecordPCMData();
                    ErrorContainer.saveNormalPCM(mRecordPCMData, para_length);
                    mRecordState = RECORD_STATE_CAPTURING;
                    break;
                case MsgCLanguage.CMD_RECORD_CLOSED:
                    mRecordState = RECORD_STATE_CLOSED;
                    Console.WriteLine("CMD_RECORD_CLOSED received");
                    break;
            }
        }

        public void registerVolumeDBUpdateListener(VolumeDBUpdateListener listener)
        {
            mVolumeDBUpdateListener = listener;
        }

        public void registerErrorReportListener(ErrorReportListener listener)
        {
            mErrorReportListener = listener;
        }

        public void setAlarmLimit(int minLimit, int maxLimit)
        {
            mMinAlarmValue = minLimit;
            mMaxAlarmValue = maxLimit;
        }

        public int getMinAlarmValue()
        {
            return mMinAlarmValue;
        }

        public int getMaxAlarmValue()
        {
            return mMaxAlarmValue;
        }

        [DllImport("ssc_core.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void register_C_msg_callback_fromdll(CallbackDelegate callback);
    }
}
