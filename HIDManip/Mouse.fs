namespace HIDManip.Mouse
open HIDManip.WinApi
open HIDManip.WinApi.Methods
open System.Runtime.InteropServices

 type MOUSEEVENT =
    | ABSOLUTE = 0x8000
    | MOVE = 0x0001
    | LEFTDOWN = 0x0002
    | LEFTUP = 0x0002
    | MIDDLEDOWN = 0x0020
    | MIDDLEUP = 0x0040
    | RIGHTDOWN = 0x0008
    | RIGHTUP = 0x0010
    | XDOWN = 0x0080
    | XUP = 0x0100
    | WHEEL = 0x0800
    | HWHEEL = 0x01000


module Hook =

    type LowLevelMouseProc =
        delegate of int * WM * byref<MSLLHOOKSTRUCT> -> HHOOK
    // Sadly I can't marshal discriminated unions, otherwise I could have a HOOKPROC that could cover all the hook types
    // As it is, I'm making distinct methods to marshal each type separately
    [<DllImport("user32.dll", SetLastError = true)>]
    extern HHOOK CallNextHookEx(HHOOK hhk, int nCode, WM wParam, [<In>]MSLLHOOKSTRUCT& lParam)
    [<DllImport("user32.dll", SetLastError = true, EntryPoint="SetWindowsHookEx")>]
    extern nativeint SetWindowsHookEx(WH hookType, LowLevelMouseProc proc,  HINSTANCE hMod, DWORD threadId)

    type LowLevelMouseHook(handler) =
        let proc = LowLevelMouseProc (fun nCode wParam lParam ->

            if nCode < 0 then
                // CallNextHook seems to raise error code 6 (handle invalid).
                // This doesn't seem to affect the hook though.
                CallNextHookEx (0n, nCode, wParam, &lParam) //|> RaiseWin32Err
            else
                let callNext = handler nCode wParam lParam
                if callNext then
                    CallNextHookEx (0n, nCode, wParam, &lParam) // |> RaiseWin32Err
                else
                    -1n // non-zero stops the event from propagating
            )
        let hookId = SetWindowsHookEx(WH.MOUSE_LL, proc, (GetModuleHandle (null)), 0u) |> RaiseWin32Err
        member this.HookId = hookId
        member this.Handler = proc // need to stop the proc from being garbage collected (the let binding is being put into the constructor)
        interface System.IDisposable with
            member this.Dispose () =            
                UnhookWindowsHookEx (this.HookId) 
                |> RaiseWin32Err
                |> ignore

    type MouseObservable () =
        let mouseEvent = new Event<WM> ()
        let hook =  new LowLevelMouseHook(fun nCode wParam lParam ->
                mouseEvent.Trigger(wParam) |> ignore
                true
            )
        member this.MouseEvent = mouseEvent.Publish
        member this.Hook = hook

module Send =
     [<DllImport("user32.dll", SetLastError = true)>]
     extern void mouse_event(MOUSEEVENT dwFlags, DWORD dx, DWORD dy, DWORD dwData, nativeint dwExtraInfo)

     let SendMouse flags dx dy data =
        mouse_event(flags, dx, dy, data, 0n)