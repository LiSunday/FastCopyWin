using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FastCopyWin.DeviceUtils;

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

        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            int left;
            int top;
            int right;
            int bottom;
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

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("User32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll")]
        public static extern int GetSystemMetrics(int hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


        public static GUITHREADINFO? GetGuiThreadInfo(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                uint threadId = GetWindowThreadProcessId(hwnd, IntPtr.Zero);
                GUITHREADINFO guiThreadInfo = new GUITHREADINFO();
                guiThreadInfo.cbSize = Marshal.SizeOf(guiThreadInfo);
                if (GetGUIThreadInfo(threadId, ref guiThreadInfo) == false)
                    return null;
                return guiThreadInfo;
            }
            return null;
        }

    }
}
