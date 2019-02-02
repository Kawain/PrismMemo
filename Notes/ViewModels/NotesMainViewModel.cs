using Notes.Models;
using Notes.Views;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Notes.ViewModels
{
    public class NotesMainViewModel : BindableBase, IRegionMemberLifetime
    {

        //Viewのインスタンスを破棄はfalse
        public bool KeepAlive { get; private set; } = false;

        //IRegionManager
        private readonly IRegionManager _regionManager;

        //NavigationParameters
        private int _naviParam;
        public int NaviParam
        {
            get { return _naviParam; }
            set { SetProperty(ref _naviParam, value); }
        }

        //検索文字
        private string _searchText1;
        public string SearchText1
        {
            get { return _searchText1; }
            set { SetProperty(ref _searchText1, value); }
        }

        //コンボボックス用
        private ObservableCollection<CategoryModel> _categoryModelsList;
        public ObservableCollection<CategoryModel> CategoryModelsList
        {
            get { return _categoryModelsList; }
            set { SetProperty(ref _categoryModelsList, value); }
        }

        //コンボボックス1つ
        private CategoryModel _selectedValue;
        public CategoryModel SelectedValue
        {
            get { return _selectedValue; }
            set { SetProperty(ref _selectedValue, value); }
        }

        //DataGrid用
        private ObservableCollection<TopModel> _topModelsList;
        public ObservableCollection<TopModel> TopModelsList
        {
            get { return _topModelsList; }
            set { SetProperty(ref _topModelsList, value); }
        }

        //DataGrid選択用
        private TopModel _item;
        public TopModel Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        //リロード
        public DelegateCommand<object> ReloadCommand { get; private set; }
        //新規追加
        public DelegateCommand AddCommand { get; private set; }
        //カテゴリ
        public DelegateCommand CateCommand { get; private set; }
        //DataGridクリック
        public DelegateCommand LeftClickCommand { get; private set; }
        //検索
        public DelegateCommand SearchCommand1 { get; private set; }
        //コンボボックスセレクト
        public DelegateCommand SelectionChangedCommand { get; private set; }

        //コンストラクタ
        public NotesMainViewModel(IRegionManager regionManager)
        {

            //IRegionManager
            _regionManager = regionManager;

            //コンボボックス用
            CategoryModelsList = SingleCategoryList.Instance;

            //DataGrid用
            TopModelsList = SQL.TopModels();

            //リロード
            ReloadCommand = new DelegateCommand<object>(ExecuteReload);
            //新規追加
            AddCommand = new DelegateCommand(ExecuteAdd);
            //カテゴリ
            CateCommand = new DelegateCommand(ExecuteCate);
            //DataGridクリック
            LeftClickCommand = new DelegateCommand(ExecuteLeftClick);
            //検索
            SearchCommand1 = new DelegateCommand(SearchExecute1);
            //コンボボックスセレクト
            SelectionChangedCommand = new DelegateCommand(SelectExecute);



        }


        //コンボボックスセレクト
        private void SelectExecute()
        {
            if (SelectedValue != null)
            {
                TopModelsList = SQL.CategorySelect(SelectedValue.Id);
            }
        }

        //リロード
        private void ExecuteReload(object o)
        {
            _regionManager.Regions["ContentRegion1"].RemoveAll();
            _regionManager.RequestNavigate("ContentRegion1", "NotesMainView");
        }

        //新規追加
        private void ExecuteAdd()
        {
            //ナビゲーションのパラメータ 0 は新規追加
            _regionManager.RequestNavigate("ContentRegion2", "FormView", new NavigationParameters("FormViewId=0"));
        }

        //カテゴリ
        private void ExecuteCate()
        {
            _regionManager.RequestNavigate("ContentRegion2", "CategoryView");
        }

        //DataGridクリック
        private void ExecuteLeftClick()
        {
            //ナビゲーションのパラメータ Id は編集
            _regionManager.RequestNavigate("ContentRegion2", "FormView", new NavigationParameters($"FormViewId={Item.Id}"));
        }

        //検索処理
        private void SearchExecute1()
        {
            TopModelsList = SQL.KeywordSelect(SearchText1);
        }
    }
}
