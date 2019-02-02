using Prism.Regions;
using PrismMemo.Properties;
using System.ComponentModel;
using System.Windows;

namespace PrismMemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();


            //https://mseeeen.msen.jp/recover-window-bounds-with-wpf/
            // ウィンドウのサイズを復元
            RecoverWindowBounds();


            //view discovery
            //https://github.com/PrismLibrary/Prism-Samples-Wpf/tree/master/04-ViewDiscovery/ViewDiscovery
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(Title));

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // ウィンドウのサイズを保存
            SaveWindowBounds();
            base.OnClosing(e);

            //これがないと完全には閉じない
            Application.Current.Shutdown();

        }

        /// <summary>
        /// ウィンドウの位置・サイズを保存します。
        /// ユーザー設定は、ユーザーの非表示のローカル アプリケーション データ フォルダーのサブフォルダー内のファイルに保存されます。
        /// 例　C:\Users\user\AppData\Local\PrismMemo
        /// </summary>
        void SaveWindowBounds()
        {
            var settings = Settings.Default;
            settings.WindowMaximized = WindowState == WindowState.Maximized;
            WindowState = WindowState.Normal; // 最大化解除
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            settings.FontSize = (int)FontSize;
            settings.Save();
        }

        /// <summary>
        /// ウィンドウの位置・サイズを復元します。
        /// </summary>
        void RecoverWindowBounds()
        {
            var settings = Settings.Default;
            // 左
            if (settings.WindowLeft >= 0 && (settings.WindowLeft + settings.WindowWidth) < SystemParameters.VirtualScreenWidth)
            {
                Left = settings.WindowLeft;
            }
            // 上
            if (settings.WindowTop >= 0 && (settings.WindowTop + settings.WindowHeight) < SystemParameters.VirtualScreenHeight)
            {
                Top = settings.WindowTop;
            }
            // 幅
            if (settings.WindowWidth > 0 && settings.WindowWidth <= SystemParameters.WorkArea.Width)
            {
                Width = settings.WindowWidth;
            }
            // 高さ
            if (settings.WindowHeight > 0 && settings.WindowHeight <= SystemParameters.WorkArea.Height)
            {
                Height = settings.WindowHeight;
            }
            // 最大化
            if (settings.WindowMaximized)
            {
                // ロード後に最大化
                Loaded += (o, e) => WindowState = WindowState.Maximized;
            }            
        }
    }
}
