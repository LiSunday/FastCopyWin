using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FastCopyWin
{
    public class Win32Api
    {
        // 处理事件委托.
        public delegate int HookHandle(int nCode, int wParam, IntPtr lParam);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        // 设置钩子
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookHandle lpfn, IntPtr hInstance, int threadId);

        // 取消钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 调用下一个钩子
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        // 获取当前线程ID
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        // Gets the main module for the associated process.
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);


        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
}
