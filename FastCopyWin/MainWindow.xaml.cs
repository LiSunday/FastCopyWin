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

        public ClipboardDataViewModel dynamicDataViewModel;
        public ClipboardDataViewModel constDataViewModel;


        public MainWindow()
        {
            // 初始化界面组件
            InitializeComponent();
            dynamicDataViewModel = new ClipboardDataViewModel();
            constDataViewModel = new ClipboardDataViewModel();
            constDataViewModel.AddFirstData("酸辣肉卤拌面");
            constDataViewModel.AddFirstData("至尊红烧牛肉面");
            constDataViewModel.AddFirstData("绝香味美咖喱");
            constDataViewModel.AddFirstData("超级过瘾排骨压土豆");
            constDataViewModel.AddFirstData("全麦奶酪三明治");

            lvClipboard.DataContext = dynamicDataViewModel;
            lvClipboard.ItemsSource = dynamicDataViewModel.CollectionView;

            // 设置主界面样式
            this.Title = "Biu~";
            float w = 0, h = 0;
            DeviceUtils.GetDeviceCaps(ref w, ref h);
            this.Height = h - (h / GOLDEN_RATIO_COEFFICIENT);
            this.Width = GOLDEN_RATIO_COEFFICIENT * this.Height - this.Height;

            // 快捷键 显示动态数据界面
            KeyboardUtils.AddKeyboardAction(KeyboardUtils.ControlKeyEnum.CTRL, KeyboardUtils.KeyEnum.M, ShowDynamicWindow);
            // 快捷键 显示常量数据界面
            KeyboardUtils.AddKeyboardAction(KeyboardUtils.ControlKeyEnum.CTRL, KeyboardUtils.KeyEnum.B, ShowConstWindow);
            // 快捷键 将数据复制到剪切板
            KeyboardUtils.AddKeyboardAction(KeyboardUtils.KeyEnum.ENTER, CoverClipboardData);
            // 鼠标点击在非界面上 则隐藏界面
            MouseUtils.AddMouseAction(MouseUtils.KeyEnum.LEFT_KEY, HideWindow);
            // ESC 也退出界面
            KeyboardUtils.AddKeyboardAction(KeyboardUtils.KeyEnum.ESC, HideWindow);
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
                dynamicDataViewModel.AddUnlikeFirstData(clipboardData);
            }
        }

        private void CoverClipboardData()
        {
            if (!this.IsVisible) return;
            ClipboardListenerService.GetInstance().SetDataObject(lvClipboard.SelectedItem);
            HideWindow();
        }

        private void ShowWindow(bool isSelectFirst) 
        {
            if (this.IsVisible) return;
            MouseUtils.MousePoint point = MouseUtils.GetCursorPosition();
            double scale = DeviceUtils.GetPositionScale();
            this.Left = point.x * scale;
            this.Top = point.y * scale;
            this.Show();
            Keyboard.Focus(this);
            Keyboard.Focus(lvClipboard);
            if (isSelectFirst)
            {
                lvClipboard.SelectedIndex = 0;
            }
        }

        private void ShowConstWindow()
        {
            lvClipboard.DataContext = constDataViewModel;
            lvClipboard.ItemsSource = constDataViewModel.CollectionView;
            ShowWindow(false);
        }

        private void ShowDynamicWindow()
        {
            lvClipboard.DataContext = dynamicDataViewModel;
            lvClipboard.ItemsSource = dynamicDataViewModel.CollectionView;
            ShowWindow(true);
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
