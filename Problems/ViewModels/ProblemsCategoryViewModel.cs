using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using Problems.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Problems.ViewModels
{
    public class ProblemsCategoryViewModel : BindableBase, IRegionMemberLifetime
    {

        //Viewのインスタンスを破棄はfalse
        public bool KeepAlive { get; private set; } = false;

        //IRegionManager
        private readonly IRegionManager _regionManager;

        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                SetProperty(ref _isEnabled, value);
            }
        }

        //カテゴリのコレクション
        private ObservableCollection<CategoryModel> _categoryModelsList;
        public ObservableCollection<CategoryModel> CategoryModelsList
        {
            get { return _categoryModelsList; }
            set { SetProperty(ref _categoryModelsList, value); }
        }

        //DataGridにバインドしたObservableCollectionの中身の変更を知る方法
        //xamlにSelectedItem="{Binding SelectedItem}"を記入して
        //PropertyChangedイベントハンドラーにイベント追加
        private CategoryModel _selectedItem;
        public CategoryModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SelectedItem != null)
                {
                    SelectedItem.PropertyChanged -= SelectedItem_PropertyChanged;
                }

                SetProperty(ref _selectedItem, value);

                if (SelectedItem != null)
                {
                    SelectedItem.PropertyChanged += SelectedItem_PropertyChanged;
                }
            }
        }

        //ObservableCollectionが追加、削除された場合
        private void CategoryModelsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsEnabled = true;
        }

        //ObservableCollectionの中身<CategoryModel>が変更された場合
        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsEnabled = true;
        }

        //メッセージボックス用
        //25-NotificationRequest
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        //コンストラクタ
        public ProblemsCategoryViewModel(IRegionManager regionManager)
        {

            //IRegionManager
            _regionManager = regionManager;

            CategoryModelsList = SingleCategoryList.Instance;

            CategoryModelsList.CollectionChanged += CategoryModelsList_CollectionChanged;

            //ObservesPropertyでIsEnabledの変更を監視
            SaveCommand = new DelegateCommand(ExecuteSave, CanExecute).ObservesProperty(() => IsEnabled);

            NotificationRequest = new InteractionRequest<INotification>();

        }

        public DelegateCommand SaveCommand { get; private set; }

        private void ExecuteSave()
        {
            if (SQL.CategorySave(CategoryModelsList))
            {
                NotificationRequest.Raise(new Notification { Content = "保存しました", Title = "成功" });
                _regionManager.Regions["ContentRegion1"].RemoveAll();
                _regionManager.RequestNavigate("ContentRegion1", "EditMenuView");
            }
            else
            {
                NotificationRequest.Raise(new Notification { Content = "保存に失敗しました", Title = "失敗" });
            }
        }

        private bool CanExecute()
        {
            return IsEnabled;
        }
    }
}
