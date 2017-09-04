// hook.cpp

#define	HOOK_API_EXPORTS
#include "hook.h"


#pragma comment(linker, "/section:.sharedata,rws")
#pragma data_seg(".sharedata")

HWND	g_hWndMain = NULL;
HWND	g_hWndInputSwitch = NULL;
UINT	g_uMsgInputSwitch = 0;
UINT	g_uMsgGetImmStatus = 0;

#pragma data_seg()

const TCHAR SZ_CAPTION_INPUT_SWITCH[] = _T("Input Switch Internal");
const TCHAR SZ_CLASS_INPUT_SWITCH[] = _T("Input Switch Message Worker Window");
const TCHAR SZ_MSG_INPUT_SWITCH[] = _T("Shell_InputSwitch_ExtWindowMessage");
const TCHAR SZ_MSG_GET_IMM_STATUS[] = _T("Pok3rLed$GetImmStatus");

HINSTANCE g_hDll;
HHOOK	g_hHook;


BOOL WINAPI DllMain(HINSTANCE hinstDll, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason == DLL_PROCESS_ATTACH) {
		g_hDll = hinstDll;
	}
	return TRUE;
}


void PostImmStatus(HWND hWnd)
{
	HWND hWndFocus = GetFocus();
	if (hWndFocus == NULL) {
		hWndFocus = hWnd;
	}

	auto open = BOOL(FALSE);
	auto conversion = DWORD(0);
	auto sentense = DWORD(0);
	auto hIMC = ImmGetContext(hWndFocus);
	if (hIMC != NULL) {
		open = ImmGetOpenStatus(hIMC);
		if (open) {
			ImmGetConversionStatus(hIMC, &conversion, &sentense);
		}
		ImmReleaseContext(hWndFocus, hIMC);
	}
	PostMessage(g_hWndMain, WM_USER + 1, open, conversion);
}


LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode != HC_ACTION) {
		return CallNextHookEx(g_hHook, nCode, wParam, lParam);
	}
	if (wParam != PM_REMOVE) {
		return CallNextHookEx(g_hHook, nCode, wParam, lParam);
	}
	const MSG& msg = *reinterpret_cast<MSG*>(lParam);
	if (msg.hwnd == g_hWndInputSwitch) {
		if ((msg.message == g_uMsgInputSwitch) && (msg.wParam == 2)) {
			PostGetImmStatus();
		}
	}
	else {
		if (msg.message == g_uMsgGetImmStatus) {
			PostImmStatus(msg.hwnd);
		}
	}

	return CallNextHookEx(g_hHook, nCode, wParam, lParam);
}


HOOK_API BOOL __stdcall InstallHook(HWND hWndMain)
{
	g_hWndMain = hWndMain;

	g_hWndInputSwitch = FindWindow(SZ_CLASS_INPUT_SWITCH, SZ_CAPTION_INPUT_SWITCH);
	if (g_hWndInputSwitch == NULL) {
		return FALSE;
	}

	g_uMsgInputSwitch = RegisterWindowMessage(SZ_MSG_INPUT_SWITCH);
	g_uMsgGetImmStatus = RegisterWindowMessage(SZ_MSG_GET_IMM_STATUS);

	g_hHook = SetWindowsHookEx(WH_GETMESSAGE, GetMsgProc, g_hDll, 0);
	return TRUE;
}


HOOK_API VOID __stdcall UninstallHook()
{
	if (g_hHook != NULL) {
		UnhookWindowsHookEx(g_hHook);
		g_hHook = NULL;
	}
}


HOOK_API VOID __stdcall PostGetImmStatus()
{
	HWND hWndForeg = GetForegroundWindow();
	if (hWndForeg == NULL){
		return;
	}
	PostMessage(hWndForeg, g_uMsgGetImmStatus, 0, 0);
}