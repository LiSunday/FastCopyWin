using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using HookHandle = FastCopyWin.Win32Api.HookHandle;

namespace FastCopyWin
{
    internal class KeyboardUtils
    {
        // 设置为静态变量防止被回收而导致函数异常
        private static HookHandle keyboardHookHandle = null;
        private static IntPtr hookWindowPtr = IntPtr.Zero;

        // 接收 SetWindowsHookEx 返回值
        private static int hHookValue = 0;
        // 组合触发键的动作 list
        private static List<CombinationKey> combinationKeyList = new();

        public static void AddKeyboardAction(ControlKeyEnum controlKeyEnum, KeyEnum keyEnum, Action keyboardAction)
        {
            combinationKeyList.Add(new CombinationKey(controlKeyEnum, keyEnum, keyboardAction));

            // 如果没有安装钩子 则安装一下
            if (hHookValue == 0)
            {
                keyboardHookHandle = new HookHandle(GetHookProc);
                hookWindowPtr = Win32Api.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                hHookValue = Win32Api.SetWindowsHookEx(
                    13,
                    keyboardHookHandle,
                    hookWindowPtr,
                    0);
            }
        }

        //钩子事件内部调用,调用_clientMethod方法转发到客户端应用。
        private static int GetHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //转换结构
                HookStruct hookStruct = (HookStruct)Marshal.PtrToStructure(lParam, typeof(HookStruct));
                foreach(CombinationKey combinationKey in combinationKeyList) 
                {
                    if (hookStruct.vkCode == (int)combinationKey.keyEnum && (int)Control.ModifierKeys == (int)combinationKey.controlKeyEnum)
                    {
                        combinationKey.keyboardAction();
                    }
                }
            }
            return Win32Api.CallNextHookEx(hHookValue, nCode, wParam, lParam);
        }

        //Hook结构
        [StructLayout(LayoutKind.Sequential)]
        public class HookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public enum KeyEnum
        {
            M = (int)Keys.M
        }

        public enum ControlKeyEnum
        { 
            ALT = (int)Keys.Alt,
            CTRL = (int)Keys.Control
        }

        private class CombinationKey
        {
            public ControlKeyEnum controlKeyEnum;
            public KeyEnum keyEnum;
            public Action keyboardAction;

            public CombinationKey(ControlKeyEnum controlKeyEnum, KeyEnum keyEnum, Action keyboardAction)
            {
                this.controlKeyEnum = controlKeyEnum;
                this.keyEnum = keyEnum;
                this.keyboardAction = keyboardAction;
            }
        }
    }
}
