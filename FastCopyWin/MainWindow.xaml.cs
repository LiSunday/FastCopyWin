using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FastCopyWin
{

    public partial class MainWindow : Window
    {
        const float GOLDEN_RATIO_COEFFICIENT = 1.618f;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            ClipboardListenerService.GetInstance().AddClipboardListener(source, ClipboardChangeData);
        }

        public ClipboardDataViewModel clipboardDataViewModel;
        public MainWindow()
        {
            // 初始化界面组件
            InitializeComponent();
            clipboardDataViewModel = new ClipboardDataViewModel();
            lvClipboard.DataContext = clipboardDataViewModel;
            lvClipboard.ItemsSource = clipboardDataViewModel.CollectionView;

            // 设置主界面样式
            float w = 0, h = 0;
            DeviceUtils.GetDeviceCaps(ref w, ref h);
            this.Height = h - (h / GOLDEN_RATIO_COEFFICIENT);
            this.Width = GOLDEN_RATIO_COEFFICIENT * this.Height - this.Height;

            // 快捷键 显示界面
            // TODO SundayLi 释放快捷键的时候才触发回调
            KeyboardUtils.AddKeyboardAction(KeyboardUtils.ControlKeyEnum.CTRL, KeyboardUtils.KeyEnum.M, ShowWindow);
            KeyboardUtils.AddKeyboardAction(KeyboardUtils.ControlKeyEnum.CTRL, KeyboardUtils.KeyEnum.ENTER, CoverClipboardData);
            // 鼠标点击在非界面上 则隐藏界面
            MouseUtils.AddMouseAction(MouseUtils.KeyEnum.LEFT_KEY, HideWindow);
        }

        void ClipboardChangeData(ClipboardListenerService.DataType type, Object data) 
        {
            Object? clipboardData = null;
            switch (type) 
            {
                case ClipboardListenerService.DataType.IMAGE:
                    if (data is BitmapSource bitmapSource)
                    {
                        // 深拷贝图像同时把透明度修改为不透明
                        bitmapSource = SundayLiUtils.CreateByOpacityColor(bitmapSource);
                        Image image = new() { Source = bitmapSource };
                        clipboardData = image;
                    }
                    break;
                case ClipboardListenerService.DataType.TEXT:
                    clipboardData = data;
                    break;
                default:
                    clipboardData = data.ToString();
                    break;
            }
            if (clipboardData != null) 
            {
                clipboardDataViewModel.AddFirstData(clipboardData);
            }
        }

        private void CoverClipboardData()
        {
            // TODO SundayLi 无法回写图像到剪切板
            ClipboardListenerService.GetInstance().SetDataObject(lvClipboard.SelectedItem);
            HideWindow();
        }

        private void ShowWindow() 
        {
            if (this.IsVisible) return;
            MouseUtils.MousePoint point = MouseUtils.GetCursorPosition();
            double scale = DeviceUtils.GetPositionScale();
            this.Left = point.x * scale;
            this.Top = point.y * scale;
            this.Show();
            Keyboard.Focus(this);
            Keyboard.Focus(lvClipboard);
        }

        private void HideWindow()
        {
            if (!this.IsMouseOver)
            {
                this.Hide();
            }
        }
    }
}
