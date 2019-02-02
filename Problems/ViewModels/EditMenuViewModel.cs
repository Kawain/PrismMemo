using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Problems.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Problems.ViewModels
{
    public class EditMenuViewModel : BindableBase, IRegionMemberLifetime
    {
        //Viewのインスタンスを破棄はfalse
        public bool KeepAlive { get; private set; } = false;

        //IRegionManager
        private readonly IRegionManager _regionManager;

        //検索文字
        private string _searchText1;
        public string SearchText1
        {
            get { return _searchText1; }
            set { SetProperty(ref _searchText1, value); }
        }

        //ListBox用
        private ObservableCollection<QuestionModel> _questionItems;
        public ObservableCollection<QuestionModel> QuestionItems
        {
            get { return _questionItems; }
            set { SetProperty(ref _questionItems, value); }
        }

        //ListBox一行
        private QuestionModel _selectedItem;
        public QuestionModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        //検索
        public DelegateCommand SearchCommand1 { get; private set; }

        //新規入力
        public DelegateCommand InsertCommand { get; private set; }

        //カテゴリ編集
        public DelegateCommand CateCommand { get; private set; }

        //リロード
        public DelegateCommand ReloadCommand { get; private set; }

        //リスト選択
        public DelegateCommand ShowCommand { get; private set; }

        //コンストラクタ
        public EditMenuViewModel(IRegionManager regionManager)
        {
            //IRegionManager
            _regionManager = regionManager;

            //ListBox用
            QuestionItems = SQL.QuestionModels(SingleCategoryList.Instance);

            //検索
            SearchCommand1 = new DelegateCommand(SearchExecute1);

            //新規入力
            InsertCommand = new DelegateCommand(ExecuteInsert);

            //カテゴリ編集
            CateCommand = new DelegateCommand(ExecuteCate);

            //リロード
            ReloadCommand = new DelegateCommand(ExecuteReload);

            //リスト選択
            ShowCommand = new DelegateCommand(ExecuteShow);

        }

        //リロード
        private void ExecuteReload()
        {
            QuestionItems = SQL.QuestionModels(SingleCategoryList.Instance);
            SearchText1 = "";
        }

        //検索
        private void SearchExecute1()
        {
            QuestionItems = SQL.SearchQuestionModels(SingleCategoryList.Instance, SearchText1);
        }

        //リスト選択
        private void ExecuteShow()
        {
            //Console.WriteLine(SelectedItem.Content);
            //ナビゲーションのパラメータ
            var parameters = new NavigationParameters();
            parameters.Add("parameter", SelectedItem);
            _regionManager.RequestNavigate("ContentRegion2", "EditFormView", parameters);
        }

        //新規入力
        private void ExecuteInsert()
        {
            _regionManager.RequestNavigate("ContentRegion2", "EditFormView");
        }

        //カテゴリ編集
        private void ExecuteCate()
        {
            _regionManager.RequestNavigate("ContentRegion2", "ProblemsCategoryView");
        }
    }
}
