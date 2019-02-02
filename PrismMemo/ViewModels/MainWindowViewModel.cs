using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using PrismMemo.Properties;
using PrismMemo.Views;

namespace PrismMemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        private readonly IRegionManager _regionManager;

        //タイトル
        private string _title = "プログラミング学習メモ";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        //ウインドウのフォントサイズ
        private int _fontSize;
        public int FontSize
        {
            get { return _fontSize; }
            set { SetProperty(ref _fontSize, value); }
        }

        //ナビゲーション
        public DelegateCommand TopCommand { get; private set; }
        public DelegateCommand MemoCommand { get; private set; }
        public DelegateCommand QACommand { get; private set; }
        public DelegateCommand BigCommand { get; private set; }
        public DelegateCommand SmallCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }


        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            // フォントサイズ読み込み
            Settings settings = Settings.Default;
            if (settings.FontSize > 0)
            {
                _fontSize = settings.FontSize;
            }
            else
            {
                _fontSize = 14;
            }

            TopCommand = new DelegateCommand(ExecuteTop);
            MemoCommand = new DelegateCommand(ExecuteMemo);
            QACommand = new DelegateCommand(ExecuteQA);
            BigCommand = new DelegateCommand(ExecuteBig);
            SmallCommand = new DelegateCommand(ExecuteSmall);
            ExitCommand = new DelegateCommand(ExecuteExit);

        }

        private void ExecuteSmall()
        {
            if (FontSize > 6)
            {
                FontSize--;
            }
        }

        private void ExecuteBig()
        {
            FontSize++;
        }

        private void ExecuteTop()
        {
            _regionManager.Regions["ContentRegion1"].RemoveAll();
            //17-BasicRegionNavigation
            _regionManager.RequestNavigate("ContentRegion1", "Title");
            _regionManager.Regions["ContentRegion2"].RemoveAll();
            _regionManager.RequestNavigate("ContentRegion2", "Description");
        }

        //メモに遷移
        private void ExecuteMemo()
        {
            _regionManager.Regions["ContentRegion1"].RemoveAll();
            _regionManager.RequestNavigate("ContentRegion1", "NotesMainView");
            _regionManager.Regions["ContentRegion2"].RemoveAll();
        }

        private void ExecuteQA()
        {
            _regionManager.Regions["ContentRegion1"].RemoveAll();
            _regionManager.RequestNavigate("ContentRegion1", "ProblemsMainView");
            _regionManager.Regions["ContentRegion2"].RemoveAll();
        }

        private void ExecuteExit()
        {
            System.Windows.Application.Current.Shutdown();
        }




    }
}
