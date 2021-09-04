#include "audio_record.h"
#include <stdlib.h>
#include <stdio.h>
#include <windows.h>
#include <conio.h>
#include <errno.h>
#include <Windows.h>
#include <stdlib.h>
#include <stdio.h>
#include "mmsystem.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "record_error.h"
#include "audio_record.h"


#define MAX_DEVCIES_SUPPORT  16

#define PERIOD_SIZE_RECORD    10240
HWAVEIN gHandleRecordDevice;
WAVEHDR gRecordWaveHeader;
int gFlagRecordStop = 0;
process_C_msg_callback gCSharp_Process_C_Msg;



char gRecordDatabufferFirst[PERIOD_SIZE_RECORD] = { 0 };

typedef enum PROCESS_CMD {
	CMD_RECORD_STARTED = 0,
	CMD_RECORD_DATA_AVALIABLE,
	CMD_RECORD_CLOSED,
}PROCESS_CMD;

typedef struct RecordConfig {
	int samplerate;
	int channels;
	int format;
}RecordConfig;

int get_device_count()
{
	WAVEINCAPS tmpCaptureDeviceInfo;
	int device_count = waveInGetNumDevs();
	int tmp_device_count = device_count;
	if (device_count <= 0) {
		return NO_CAPTURE_DEVICE_FOUND;
	}

	tmp_device_count = device_count < MAX_DEVCIES_SUPPORT ? device_count : MAX_DEVCIES_SUPPORT;
	return tmp_device_count;
}

void register_C_msg_callback(process_C_msg_callback cb) {
	gCSharp_Process_C_Msg = cb;
}

char* get_device_name(int device_index) {
	WAVEINCAPS tmpCaptureDeviceInfo;
	MMRESULT result = waveInGetDevCaps(device_index, &tmpCaptureDeviceInfo, sizeof(tmpCaptureDeviceInfo));
	if (result == MMSYSERR_NOERROR) {
		write_log("audio_record###device_name:%s\n", tmpCaptureDeviceInfo.szPname);
		return tmpCaptureDeviceInfo.szPname;
	}
	return NULL;
}

int get_configs_device_support(int device_index) {
	WAVEINCAPS tmpCaptureDeviceInfo;
	MMRESULT result = waveInGetDevCaps(device_index, &tmpCaptureDeviceInfo, sizeof(tmpCaptureDeviceInfo));
	if (result == MMSYSERR_NOERROR) {
		return tmpCaptureDeviceInfo.dwFormats;
	}
	return 0;
}

DWORD CALLBACK record_data_avaliable(HWAVEIN hwavein, UINT uMsg, DWORD dwInstance, DWORD dwParam1, DWORD dwParam2)
{
	MMRESULT result;
	switch (uMsg)
	{
	case WIM_OPEN:
		write_log("audio_record###WIM_OPEN received\n");
		(*gCSharp_Process_C_Msg)(
			CMD_RECORD_STARTED,
			NULL,
			0
			);
		break;
	case WIM_DATA:
		write_log("audio_record###WIM_DATA received, buffer:%d\n", ((LPWAVEHDR)dwParam1)->dwUser);
		(*gCSharp_Process_C_Msg)(
									CMD_RECORD_DATA_AVALIABLE,
									((LPWAVEHDR)dwParam1)->lpData, 
									((LPWAVEHDR)dwParam1)->dwBytesRecorded
								);
		
		//dump_data("dump.pcm", ((LPWAVEHDR)dwParam1)->lpData, ((LPWAVEHDR)dwParam1)->dwBytesRecorded);
		if (gFlagRecordStop == 0) {
			waveInAddBuffer(hwavein, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR));
		}
		break;
	case WIM_CLOSE:
		(*gCSharp_Process_C_Msg)(
			CMD_RECORD_CLOSED,
			NULL,
			0
			);
			write_log("audio_record###WIM_CLOSE received\n");
			break;
	default:
			break;
	}
	return 0;
}

int stop_audio_record()
{
	MMRESULT result;
	write_log("audio_record###closing record device\n");
	result = waveInStop(gHandleRecordDevice);
	if (MMSYSERR_NOERROR == result) {
		write_log("audio_record###waveInStop success\n");
	}
	gFlagRecordStop = 1;
	result = waveInReset(gHandleRecordDevice);
	if (MMSYSERR_NOERROR == result) {
		write_log("audio_record###waveInReset success\n");
	}
	result = waveInUnprepareHeader(gHandleRecordDevice, &gRecordWaveHeader, sizeof(WAVEHDR));
	if (MMSYSERR_NOERROR == result) {
		write_log("audio_record###waveInUnprepareHeader First success\n");
	}

	result = waveInClose(gHandleRecordDevice);
	if (MMSYSERR_NOERROR == result) {
		write_log("audio_record###waveInClose success\n");
	}

}

int start_audio_record(int device_selected, int config_samperate, int config_channels, int config_format, int period_size)
{
	WAVEFORMATEX recordConfig;
	MMRESULT result;
	int frame_size = config_channels * (config_format >> 3);

	recordConfig.wFormatTag = WAVE_FORMAT_PCM;
	recordConfig.nChannels = config_channels;
	recordConfig.nSamplesPerSec = config_samperate;
	recordConfig.nAvgBytesPerSec = config_samperate * frame_size;
	recordConfig.nBlockAlign = frame_size;
	recordConfig.wBitsPerSample = config_format;
	recordConfig.cbSize = 0;
	gFlagRecordStop = 0;
	write_log("audio_record###open record device, device:%d, sample:%d, channels:%d, period_size:%d, wBitsPerSample:%d\n",
				device_selected, config_samperate, config_channels, period_size, recordConfig.wBitsPerSample);
	result = waveInOpen(&gHandleRecordDevice, device_selected, &recordConfig, (DWORD)(record_data_avaliable), NULL, CALLBACK_FUNCTION);
	if (MMSYSERR_NOERROR == result) {
		gRecordWaveHeader.lpData = gRecordDatabufferFirst;
		gRecordWaveHeader.dwBufferLength = period_size;
		gRecordWaveHeader.dwUser = 1;
		gRecordWaveHeader.dwFlags = 0;
		result = waveInPrepareHeader(gHandleRecordDevice, &gRecordWaveHeader, sizeof(WAVEHDR));
		if (MMSYSERR_NOERROR != result) {
			write_log("audio_record###waveInPrepareHeader failed\n");
			goto exit;
		}
		write_log("audio_record###waveInPrepareHeader success\n");
		result = waveInAddBuffer(gHandleRecordDevice, &gRecordWaveHeader, sizeof(WAVEHDR));
		if (MMSYSERR_NOERROR != result) {
			write_log("audio_record###waveInAddBuffer failed\n");
			goto exit;
		}

		write_log("audio_record###waveInAddBuffer success\n");
		result = waveInStart(gHandleRecordDevice);
		if (MMSYSERR_NOERROR != result) {
			write_log("audio_record###waveInStart failed\n");
			goto exit;
		}
		write_log("audio_record###waveInStart success\n");
	}
	else {
		write_log("audio_record###open device failed\n");
		return OPEN_DEVICE_FAILED;
	}
exit:
	return result;
}





