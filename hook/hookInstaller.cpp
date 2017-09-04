// hookInstaller.cpp

#include <stdio.h>
#include <string.h>
#include <stdint.h>
#include "hook.h"


#if PARCH == 1
#pragma message("ARCH:x64")
const WCHAR NAME_MODULE[] = L"hookInstaller_x64";
#elif PARCH == 2
#pragma message("ARCH:x86")
const WCHAR NAME_MODULE[] = L"hookInstaller_x86";
#else
#pragma message("No ARCH is specified")
#endif
 

bool StartWith(LPWSTR pstr, LPCWSTR value)
{
	auto len = wcslen(value);
	auto r = wcsncmp(pstr, value, len);
	return r == 0;
}


HWND GetHWnd(LPCWSTR pstr)
{
	auto temp = uint32_t(0);
	for (int i = 0; i < 8; ++i) {
		wchar_t ch = pstr[i];
		temp <<= 4;
		if (('0' <= ch) && (ch <= '9')) {
			temp += ch - '0';
		}
		else if (('A' <= ch) && (ch <= 'F')) {
			temp += ch - 'A' + 10;
		}
		else if (('a' <= ch) && (ch <= 'f')) {
			temp += ch - 'a' + 10;
		}
		else {
			return NULL;
		}
	}
	return reinterpret_cast<HWND>(temp);
}



int CALLBACK WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
	OutputDebugString(NAME_MODULE);
	OutputDebugString(TEXT("\r\n"));

	wprintf(L"HELLO\r\n");
	fflush(stdout);

	while (true) {
		WCHAR buff[256];
		auto r = fgetws(buff, 256, stdin);
		if (r == NULL) {
			break;
		}
		if (StartWith(buff, L"HOOK:")) {
			auto hWnd = GetHWnd(&buff[5]);
			InstallHook(hWnd);
//			MessageBoxW(NULL, buff, NAME_MODULE, MB_OK);
		}
		else if (StartWith(buff, L"POST")) {
			PostGetImmStatus();
		}
		else if (StartWith(buff, L"EXIT")) {
			break;
		}
	}

	UninstallHook();
//	MessageBoxW(NULL, L"EXIT", NAME_MODULE, MB_OK);
    return 0;
}
