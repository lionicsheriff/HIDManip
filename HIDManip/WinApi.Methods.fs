namespace HIDManip.WinApi
open System.Runtime.InteropServices

module Methods =

    let RaiseWin32Err a' =    
        let win32Err = System.Runtime.InteropServices.Marshal.GetLastWin32Error ()    
        if win32Err <> 0 then
            let err = System.ComponentModel.Win32Exception win32Err
            raise (System.ComponentModel.Win32Exception win32Err)
        else
            a'

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern uint32 GetCurrentThreadId()

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern nativeint GetModuleHandle(string lpModuleName)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool GetMessage(MSG lpMsg, nativeint hWnd, uint32 wMsgFilterMin, uint32 wMsgFilterMax);

    [<DllImport("user32.dll", SetLastError = true)>]
    extern UINT SendInput(UINT nInputs, [<MarshalAs(UnmanagedType.LPArray); In>] INPUT[] pInputs, int cbSize);

    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool GetGUIThreadInfo(DWORD idThread, [<In; Out>]GUITHREADINFO& lpgui)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern ULONG_PTR GetMessageExtraInfo()

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern DWORD GetLastError()

    [<DllImport("user32.dll", SetLastError = true)>]
    extern uint32 MapVirtualKey(VK uCode, uint32 uMapType)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool SetForegroundWindow(HWND hwnd)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool UnhookWindowsHookEx(nativeint hhk)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern HWND GetFocus()

    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool AttachThreadInput(DWORD idAttach, DWORD idAttachTo, bool fAttach)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern DWORD GetWindowThreadProcessId(HWND hwnd, [<Out>] DWORD& lpdwProcessId)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool SendMessage(HWND hwnd, UINT msg, VK wParam, int lParam)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool PostMessage(HWND hwnd, UINT msg, VK wParam, int lParam)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern HWND FindWindow(string lpClassName, string lpWindowName)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern HWND WindowFromPoint(POINT point)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern HWND WindowFromPhysicalPoint(POINT point)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern HWND GetTopWindow(HWND hwnd)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern HWND GetForegroundWindow()

    [<DllImport("user32.dll", SetLastError = true)>]
    extern UINT GetWindowModuleFileName(HWND hwnd, [<Out>]System.Text.StringBuilder lpszFileName, UINT cchFileNameMax)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern int GetWindowText(HWND hWnd, [<Out>]System.Text.StringBuilder lpString, int nMaxCount)

     [<DllImport("user32.dll", SetLastError = true)>]
     extern int GetWindowTextLength(HWND hWnd)
     
     [<DllImport("user32.dll", SetLastError = true)>]
     extern bool LockSetForegroundWindow(UINT uLockCode)
     
     [<DllImport("user32.dll", SetLastError = true)>]
     extern HWND ChildWindowFromPoint(HWND hWndParent, POINT Point)
      
