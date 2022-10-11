using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HookHandle = FastCopyWin.Win32Api.HookHandle;
using POINT = FastCopyWin.Win32Api.POINT;

namespace FastCopyWin
{
    internal class MouseUtils
    {
        // 设置为静态变量防止被回收而导致函数异常
        private static HookHandle mouseHookHandle = new HookHandle(GetHookProc);
        private static IntPtr hookWindowPtr = IntPtr.Zero;
        // 接收 SetWindowsHookEx 返回值
        private static int hHookValue = 0;

        private static List<KeyAction> keyActionList = new();
        public static void AddMouseAction(KeyEnum key, Action action)
        {
            keyActionList.Add(new KeyAction(key, action));
            if (hHookValue == 0)
            {
                hookWindowPtr = Win32Api.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                hHookValue = Win32Api.SetWindowsHookEx(
                    14,
                    mouseHookHandle,
                    hookWindowPtr,
                    0);
                Debug.WriteLine("鼠标监听挂载成功！" + hHookValue);
            }
        }
        private static int GetHookProc(int nCode, int wParam, IntPtr lParam)
        {
            MouseHookStruct mouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            if (nCode >= 0)
            { 
                foreach (KeyAction keyAction in keyActionList)
                {
                    if ((int)keyAction.key == wParam)
                    {
                        keyAction.action();
                    }
                }
            }
            return Win32Api.CallNextHookEx(hHookValue, nCode, wParam, lParam);
        }

        public static MousePoint GetCursorPosition()
        {
            POINT point;
            Win32Api.GetCursorPos(out point);
            return new MousePoint(point.x, point.y);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public enum KeyEnum 
        {
            LEFT_KEY = 0x201
        }

        private class KeyAction
        {
            public KeyEnum key;
            public Action action;

            public KeyAction(KeyEnum key, Action action)
            {
                this.key = key;
                this.action = action;
            }
        }

        public class MousePoint
        {
            public int x;
            public int y;
            public MousePoint(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
