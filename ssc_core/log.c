#pragma once
#include <stdio.h>  
#include <stdarg.h>  
#include <time.h>  

#define LOG_NAME "ssc.log"

void write_log(const char* format, ...) 
{
    FILE* pFile = NULL;
    va_list arg;

    fopen_s(&pFile, LOG_NAME, "a+");
    if (NULL == pFile)
        return;
    va_start(arg, format);

    time_t time_log = time(NULL);
    struct tm tm_log;
    localtime_s(&tm_log, &time_log);
    fprintf(pFile, "%04d-%02d-%02d %02d:%02d:%02d ", tm_log.tm_year + 1900, tm_log.tm_mon + 1, tm_log.tm_mday, tm_log.tm_hour, tm_log.tm_min, tm_log.tm_sec);

    vfprintf(pFile, format, arg);
    va_end(arg);

    fflush(pFile);

    fclose(pFile);
    return;
}
