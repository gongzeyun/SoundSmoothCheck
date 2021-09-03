#pragma once
#include <stdio.h>  
#include <stdarg.h>  
#include <time.h>  


void dump_data(char* path, char* data, int size) {
    FILE* pFile = NULL;
    va_list arg;

    fopen_s(&pFile, path, "ab+");
    if (NULL == pFile)
        return;

    fwrite(data, size, 1, pFile);

    fflush(pFile);

    fclose(pFile);
    return;
}
