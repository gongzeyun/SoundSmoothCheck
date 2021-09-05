using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SoundCheck
{
    class AudioRecoder
    {
        List<KeyValuePair<int, RecordConfigs>> mRecordConfigs = new List<KeyValuePair<int, RecordConfigs>>();
        public const int MSG_RECORD_COMPLETELY = 0;
        public const int MSG_ERROR_REPORTED = 1;
        public const int MSG_UPDATE_VOLUME_POINT = 2;

        private int mSelectedDevice = 0;
        private int mSelectedConfig = 0;
        private static Int64 mRecordSampleSizeSum = 0;
        private CallbackDelegate mCallBackFunction;

        public static int RECORD_STATE_OPENED = 0;
        public static int RECORD_STATE_CAPTURING = 1;
        public static int RECORD_STATE_CLOSED = 2;

        private int mRecordState = RECORD_STATE_CLOSED;

        private int mMinAlarmValue;
        private int mMaxAlarmValue;

        private ErrorContainer mErrorContainer = null;

        private int mSecondsRecordDuration;

        private UIOwner mUIOwner;

        public int getRecordState()
        {
            return mRecordState;
        }
        public static int getDeviceCount()
        {
            return get_device_count_fromdll();
        }

        public AudioRecoder()
        {
            mCallBackFunction = CallBackFromCLanuage;
            register_C_msg_callback_fromdll(mCallBackFunction);
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000100, new RecordConfigs(44100, 1, 8, "44100, Mono, 8bit", 4096)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000200, new RecordConfigs(44100, 2, 8, "44100, Stereo, 8bit", 4096)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000400, new RecordConfigs(44100, 1, 16, "44100, Mono, 16bit", 4096)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00000800, new RecordConfigs(44100, 2, 16, "44100, Stereo, 16bit", 4096)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00001000, new RecordConfigs(48000, 1, 8, "48000, Mono, 8bit", 4096)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00002000, new RecordConfigs(48000, 2, 8, "48000, Stereo, 8bit", 4096)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00004000, new RecordConfigs(48000, 1, 16, "48000, Mono, 16bit", 4096)));
            mRecordConfigs.Add(new KeyValuePair<int, RecordConfigs>(0x00008000, new RecordConfigs(48000, 2, 16, "48000, Stereo, 16bit", 4096)));
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
                recordConfigSelected.mPeriodSize);
        }

        public void processRecordPCMData(byte[] pcm_data)
        {
            double volumeDB = Tools.getVolumeDB(pcm_data, pcm_data.Length);
            Int64 timeMS = Tools.getRecordTime(mRecordConfigs[mSelectedConfig].Value, mRecordSampleSizeSum);
            if (timeMS > mSecondsRecordDuration * 1000)
            {
                mUIOwner.UpdateUIAccordMsg(AudioRecoder.MSG_RECORD_COMPLETELY, null);
                return;
            }
            mUIOwner.UpdateUIAccordMsg(AudioRecoder.MSG_UPDATE_VOLUME_POINT, new TimeAndVolumeDBPoint(timeMS, volumeDB));

            if (mErrorContainer != null) //an error is processing
            {
                if (mErrorContainer.getState() == ErrorContainer.ERROR_IS_MAKEING)
                {
                    mErrorContainer.saveErrorPCMData(pcm_data, pcm_data.Length);
                }
                else
                {
                    //notify UI thread to display Error link label
                    mUIOwner.UpdateUIAccordMsg(MSG_ERROR_REPORTED, mErrorContainer);
                    mErrorContainer = null;
                }
                return;
            }
            else //no error is reporting, start next round
            {
                if ((volumeDB < mMinAlarmValue || volumeDB > mMaxAlarmValue))
                {
                    mErrorContainer = new ErrorContainer(DateTime.Now);
                }
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
                    byte[] pcm_data = new byte[para_length];
                    Marshal.Copy(data, pcm_data, 0, para_length);
                    mRecordSampleSizeSum += para_length;
                    processRecordPCMData(pcm_data);
                    ErrorContainer.normalPCMSaved(pcm_data, para_length);
                    mRecordState = RECORD_STATE_CAPTURING;
                    break;
                case MsgCLanguage.CMD_RECORD_CLOSED:
                    mRecordState = RECORD_STATE_CLOSED;
                    Console.WriteLine("CMD_RECORD_CLOSED received");
                    break;
            }
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

        public void setRecordDuration(int recordDuration)
        {
            mSecondsRecordDuration = recordDuration;
        }

        public void registerUIOwner(UIOwner owner)
        {
            mUIOwner = owner;
        }
        [DllImport("ssc_core.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void register_C_msg_callback_fromdll(CallbackDelegate callback);
    }
}
