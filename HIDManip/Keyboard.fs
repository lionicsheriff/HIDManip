#nowarn "9"
namespace HIDManip
open HIDManip.WinApi
open HIDManip.WinApi.Methods
open System.Runtime.InteropServices

[<Struct; StructLayout(LayoutKind.Sequential)>]
type KBDLLHOOKSTRUCT =
    val vkCode: VK
    val scanCode: DWORD
    val flags: DWORD
    val time: DWORD
    val dwExtraInfo: ULONG_PTR

module Hook =

    // Sadly I can't marshal discriminated unions, otherwise I could have a HOOKPROC that could cover all the hook types
    // As it is, I'm making distinct methods to marshal each type separately
    type LowLevelKeyboardProc =
        delegate of int * WM * byref<KBDLLHOOKSTRUCT> -> HHOOK
    [<DllImport("user32.dll", SetLastError = true)>]
    extern HHOOK CallNextHookEx(HHOOK hhk, int nCode, WM wParam, [<In>]KBDLLHOOKSTRUCT& lParam)
    [<DllImport("user32.dll", SetLastError = true, EntryPoint="SetWindowsHookEx")>]
    extern nativeint SetWindowsHookEx(WH hookType, LowLevelKeyboardProc proc,  HINSTANCE hMod, DWORD threadId)

    type LowLevelKeyboardHook(handler) =
        let proc = LowLevelKeyboardProc(fun nCode wParam lParam ->
            if nCode < 0 then
                // CallNextHook seems to raise error code 6 (handle invalid) This doesn't seem consistent though, so maybe not my code?
                CallNextHookEx(0n, nCode, wParam, &lParam) //|> RaiseWin32Err
            else
                let callNext = handler nCode wParam lParam
                if callNext then
                    CallNextHookEx(0n, nCode, wParam, &lParam)  //|> RaiseWin32Err
                else
                    -1n // non-zero stops the event from propagating
            )
        let hookId = SetWindowsHookEx(WH.KEYBOARD_LL, proc, GetModuleHandle(null), 0u) |> RaiseWin32Err
        member this.HookId = hookId
        member this.Handler = proc // need to stop the proc from being garbage collected (the let binding is being put into the constructor)
        interface System.IDisposable with
            member this.Dispose() =            
                UnhookWindowsHookEx(this.HookId) 
                |> RaiseWin32Err
                |> ignore



module Send =

    [<DllImport("user32.dll", SetLastError = true)>]
    extern void keybd_event(VK bVk, uint32 bScan, KEYEVENTF dwFlags, nativeint dwExtraInfo)

    let Keys (keys: List<VK>) =
        if keys.Length = 0 then
            ()
        else        
            // I have been having issues using SendInput to send keyup, so I have switched to the slightly worse keybd_event for now
        
            // Send the keys
            for key in keys do        
                keybd_event (key, MapVirtualKey(key, 0u), KEYEVENTF.NONE, 0n)
                
            // and their corresponding key up (don't want to get into ctrl lock...)
            for key in keys do            
                keybd_event (key, MapVirtualKey(key, 0u), KEYEVENTF.KEYUP, 0n)
            
            ()