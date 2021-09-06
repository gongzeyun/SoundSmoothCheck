#include <stdio.h>  
#include <stdarg.h>  
#include <time.h>  
#include <Windows.h>

#define LOG_NAME "ssc.log"

void write_log(const char* format, ...) 
{
    FILE* pFile = NULL;
    va_list arg;

    fopen_s(&pFile, LOG_NAME, "a+");
    if (NULL == pFile)
        return;
    va_start(arg, format);
    SYSTEMTIME sys;
    GetLocalTime(&sys);
    fprintf(pFile, "%04d-%02d-%02d %02d:%02d:%02d:%04d ", sys.wYear, sys.wMonth, sys.wDay, sys.wHour, sys.wMinute, sys.wSecond, sys.wMilliseconds);

    vfprintf(pFile, format, arg);
    va_end(arg);

    fflush(pFile);

    fclose(pFile);
    return;
}
