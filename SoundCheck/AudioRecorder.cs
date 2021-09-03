using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Microsoft;
using NAudio.CoreAudioApi;
using System.Runtime.InteropServices;

namespace SoundCheck
{
    class AudioRecorder
    {
        List<KeyValuePair<int, RecordConfigs>> mRecordConfigs = new List<KeyValuePair<int, RecordConfigs>>();

        private int mSelectedDevice = 0;
        private int mSelectedConfig = 0;

        private const int mRecordPeriodSize = 4096;
        public static int getDeviceCount()
        {
            return get_device_count_fromdll();
        }

        public AudioRecorder()
        {

            register_C_msg_callback_fromdll(CallBackFromCLanuage);
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
            
            // now we check follow configs only
            //    #define WAVE_FORMAT_44M08      0x00000100       /* 44.1   kHz, Mono,   8-bit  */
            //    #define WAVE_FORMAT_44S08      0x00000200       /* 44.1   kHz, Stereo, 8-bit  */
            //    #define WAVE_FORMAT_44M16      0x00000400       /* 44.1   kHz, Mono,   16-bit */
            //    #define WAVE_FORMAT_44S16      0x00000800       /* 44.1   kHz, Stereo, 16-bit */
            //    #define WAVE_FORMAT_48M08      0x00001000       /* 48     kHz, Mono,   8-bit  */
            //    #define WAVE_FORMAT_48S08      0x00002000       /* 48     kHz, Stereo, 8-bit  */
            //    #define WAVE_FORMAT_48M16      0x00004000       /* 48     kHz, Mono,   16-bit */
            //    #define WAVE_FORMAT_48S16      0x00008000       /* 48     kHz, Stereo, 16-bit */
            //
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
            start_audio_record_fromdll(mSelectedDevice, recordConfigSelected.mSamplerate, recordConfigSelected.mChannels, recordConfigSelected.mBitFormat, mRecordPeriodSize);
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

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void CallbackDelegate(int cmd, [MarshalAs(UnmanagedType.LPArray, SizeConst = AudioRecorder.mRecordPeriodSize)] byte[] para, int para_length);

        private void CallBackFromCLanuage(int cmd, [MarshalAs(UnmanagedType.LPArray, SizeConst = AudioRecorder.mRecordPeriodSize)] byte[] para, int para_length)
        {
            switch (cmd)
            {
                case MsgCLanguage.CMD_RECORD_STARTED:
                    break;
                case MsgCLanguage.CMD_RECORD_DATA_AVALIABLE:
                    Tools.dumpRecordPCM("dumpCSharp.pcm", para, para_length);
                    break;
                case MsgCLanguage.CMD_RECORD_CLOSED:
                    break;
            }
        }

        [DllImport("ssc_core.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void register_C_msg_callback_fromdll(CallbackDelegate callback);
    }
}
