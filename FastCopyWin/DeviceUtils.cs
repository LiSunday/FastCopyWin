using System;
using System.Drawing;
using System.Windows.Forms;

namespace FastCopyWin
{
    internal class DeviceUtils
    {
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;

        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;

        /// <summary>
        /// 获取屏幕缩放比例系数
        /// </summary>
        /// <returns></returns>
        public static float GetPositionScale()
        {
            var hdc = Win32Api.GetDC(IntPtr.Zero);
            var dpiX = Win32Api.GetDeviceCaps(hdc, LOGPIXELSX);
            Win32Api.ReleaseDC(IntPtr.Zero, hdc);
            var scale = Convert.ToInt32(dpiX / 96f * 100);
            switch (scale)
            {
                case 125:
                    return .8f;
                case 150:
                    return .65f;
                case 175:
                    return .575f;
                default:
                    return 1f;
            }
        }

        /// <summary>
        /// 获取屏幕逻辑宽高
        /// </summary>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        public static void GetDeviceCaps(ref float w, ref float h)
        {
            var scale = GetPositionScale();
            w = Win32Api.GetSystemMetrics(SM_CXSCREEN) * scale; //屏幕宽度
            h = Win32Api.GetSystemMetrics(SM_CYSCREEN) * scale; //屏幕高度
        }


    }
}
