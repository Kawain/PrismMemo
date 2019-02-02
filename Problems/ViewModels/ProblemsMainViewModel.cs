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
    public class ProblemsMainViewModel : BindableBase, IRegionMemberLifetime
    {
        //Viewのインスタンスを破棄はfalse
        public bool KeepAlive { get; private set; } = false;

        //IRegionManager
        private readonly IRegionManager _regionManager;

        //ListBox用
        private ObservableCollection<CategoryModel> _items;
        public ObservableCollection<CategoryModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        //スタートボタン
        public DelegateCommand<object> StartCommand { get; private set; }

        //問題作成ボタン
        public DelegateCommand CreateCommand { get; private set; }


        //コンストラクタ
        public ProblemsMainViewModel(IRegionManager regionManager)
        {
            //IRegionManager
            _regionManager = regionManager;

            //ListBox用
            Items = SingleCategoryList.Instance;

            //スタートボタン
            StartCommand = new DelegateCommand<object>(ExecuteStart);

            //問題作成ボタン
            CreateCommand = new DelegateCommand(ExecuteCreate);
        }

        //スタートボタン
        private void ExecuteStart(object obj)
        {
            /*
             * ListBoxのSelectionMode="Multiple"では
             * 選択されたオブジェクトはSelectedItemもしくはSelectedItemsで取得できる 
             * SelectedItemプロパティは単一オブジェクトのプロパティのため、 
             * 複数オブジェクトが選択されると先頭のオブジェクトが取得される
             * 選択された複数オブジェクトの取得はSelectedItemsで取得する
             * 
             * SelectedItemsを目的のオブジェクトにキャストして使う
             */

            //コレクションにして
            var o = (System.Collections.IList)obj;

            //よくわからないのでリストを作り直す
            List<CategoryModel> list = new List<CategoryModel>();

            //一つずつキャストしてリストに入れる
            foreach (var v in o)
            {
                list.Add((CategoryModel)v);
            }

            //ナビゲーションのパラメータにリストを入れる
            var parameters = new NavigationParameters();
            parameters.Add("parameter", list);

            _regionManager.RequestNavigate("ContentRegion2", "QaView", parameters);

        }

        //問題作成ボタン
        private void ExecuteCreate()
        {
            _regionManager.RequestNavigate("ContentRegion1", "EditMenuView");
            _regionManager.RequestNavigate("ContentRegion2", "EditFormView");
        }
    }
}
