#include <stdio.h>
#include "audio_record.h"

__declspec(dllexport) void register_C_msg_callback_fromdll(process_C_msg_callback cb);

__declspec(dllexport) char* get_device_name_fromdll(const int index);
__declspec(dllexport) int get_device_count_fromdll();
__declspec(dllexport) int get_configs_device_support_fromdll(const int index);
__declspec(dllexport) int start_audio_record_fromdll(const int index, const int samplerate, const int channels, const int format, int period_size);
__declspec(dllexport) int stop_audio_record_fromdll();


void register_C_msg_callback_fromdll(process_C_msg_callback cb) {
	register_C_msg_callback(cb);
}

int get_device_count_fromdll() {
	return get_device_count();
}

char* get_device_name_fromdll(const int index) {
	return get_device_name(index);
}

int get_configs_device_support_fromdll(const int index) {
	return get_configs_device_support(index);
}

int start_audio_record_fromdll(int device_selected, int configs_samplerate, int configs_channels, int configs_format, int period_size) {
	return start_audio_record(device_selected, configs_samplerate, configs_channels, configs_format, period_size);
}

int stop_audio_record_fromdll() {
	return stop_audio_record();
}

