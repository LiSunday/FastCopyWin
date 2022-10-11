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
            // 快捷键 显示界面
            KeyboardUtils.AddKeyboardAction(KeyboardUtils.ControlKeyEnum.CTRL, KeyboardUtils.KeyEnum.M, ShowWindow);
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

        private void ShowWindow() 
        {
            MouseUtils.MousePoint point = MouseUtils.GetCursorPosition();
            // TODO SunayLi 研究一下屏幕分辨率、窗口相对位置、鼠标坐标的关系
            this.Left = point.x;
            this.Top = point.y;
            clipboardDataViewModel.AddFirstData("x : " + point.x + " y : " + point.y);
            this.Show();
            Keyboard.Focus(this);
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
