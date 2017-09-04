// hook.h

#pragma once

#include <windows.h>
#include <tchar.h>

#ifdef HOOK_API_EXPORTS
#define	HOOK_API	extern "C" __declspec(dllexport)
#else
#define HOOK_API	extern "C" __declspec(dllimport)
#endif

HOOK_API BOOL __stdcall InstallHook(HWND hWndMain);
HOOK_API VOID __stdcall UninstallHook();
HOOK_API VOID __stdcall PostGetImmStatus();
