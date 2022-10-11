using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FastCopyWin
{
    internal class ClipboardListenerService
    {
        // --- hook ---
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        // --- hook ---

        // --- var ---
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;
        private IntPtr mNextClipBoardViewerHWnd;
        private ChangeData? changeData;
        // --- var ---

        private static readonly Object lockObj = new();
        

        private static ClipboardListenerService service = null;

        public static ClipboardListenerService GetInstance()
        {
            if (service == null)
            {
                lock (lockObj)
                {
                    service ??= new ClipboardListenerService();
                }
            }
            return service;
        }

        public static Object GetClipboardData(DataType type)
        {
            // 你操作的时候可能系统正在操作 导致获取失败 这个时候快速重试就可以了
            for (int i = 0; i < 200; i++)
            {
                try
                {
                    switch (type) {
                        case DataType.TEXT:
                            return Clipboard.GetText();
                        case DataType.IMAGE:
                            return Clipboard.GetImage();
                    }
                }
                catch { }
            }
            return String.Empty;
        }

        public void AddClipboardListener(HwndSource source, ChangeData function)
        {
            mNextClipBoardViewerHWnd = SetClipboardViewer(source.Handle);
            source.AddHook(WndProc);
            changeData = function;
        }

        public delegate void ChangeData(DataType type, Object data);

        // 钩子的委托实现
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        // windows 文档要求把消息传递给窗口链的下一个
                        SendMessage(mNextClipBoardViewerHWnd, msg, wParam.ToInt32(), lParam.ToInt32());
                        if (Clipboard.ContainsText())
                        {
                            changeData(DataType.TEXT, GetClipboardData(DataType.TEXT));
                        }
                        else if (Clipboard.ContainsImage()) 
                        {
                            changeData(DataType.IMAGE, GetClipboardData(DataType.IMAGE));
                        }
                    }
                    break;
                case WM_CHANGECBCHAIN:
                    {
                        if (wParam == (IntPtr)mNextClipBoardViewerHWnd)
                        {
                            mNextClipBoardViewerHWnd = lParam;
                        }
                        else
                        {
                            SendMessage(mNextClipBoardViewerHWnd, msg, wParam.ToInt32(), lParam.ToInt32());
                        }
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        public enum DataType 
        {
            TEXT, IMAGE
        }
    }
}
