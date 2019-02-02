using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using Problems.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Problems.ViewModels
{
    public class EditFormViewModel : BindableBase, IRegionMemberLifetime, INavigationAware
    {
        //Viewのインスタンスを破棄はfalse
        public bool KeepAlive { get; private set; } = false;

        //IRegionManager
        private readonly IRegionManager _regionManager;

        //カテゴリのコレクション
        private ObservableCollection<CategoryModel> _categoryModelsList;
        public ObservableCollection<CategoryModel> CategoryModelsList
        {
            get { return _categoryModelsList; }
            set { SetProperty(ref _categoryModelsList, value); }
        }

        //新規か編集か
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        //ListBox一行
        private QuestionModel _selectedItem;
        public QuestionModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        //id
        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        //コンボボックス選択
        private CategoryModel _categoryItem;
        public CategoryModel CategoryItem
        {
            get { return _categoryItem; }
            set { SetProperty(ref _categoryItem, value); }
        }

        //問題文
        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        //正解
        private string _correct;
        public string Correct
        {
            get { return _correct; }
            set { SetProperty(ref _correct, value); }
        }

        //不正解１
        private string _incorrect1;
        public string Incorrect1
        {
            get { return _incorrect1; }
            set { SetProperty(ref _incorrect1, value); }
        }

        //不正解２
        private string _incorrect2;
        public string Incorrect2
        {
            get { return _incorrect2; }
            set { SetProperty(ref _incorrect2, value); }
        }

        //不正解３
        private string _incorrect3;
        public string Incorrect3
        {
            get { return _incorrect3; }
            set { SetProperty(ref _incorrect3, value); }
        }

        //解説
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        //新規追加ボタン有効無効
        private bool _isEnabled1;
        public bool IsEnabled1
        {
            get { return _isEnabled1; }
            set { SetProperty(ref _isEnabled1, value); }
        }

        //編集削除ボタン有効無効
        private bool _isEnabled2;
        public bool IsEnabled2
        {
            get { return _isEnabled2; }
            set { SetProperty(ref _isEnabled2, value); }
        }

        //新規追加
        public DelegateCommand AddCommand { get; private set; }

        //修正保存
        public DelegateCommand UpdateCommand { get; private set; }

        //削除
        public DelegateCommand DelCommand { get; private set; }

        //メッセージボックス用
        //25-NotificationRequest
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        //確認画面
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }

        //コンストラクタ
        public EditFormViewModel(IRegionManager regionManager)
        {

            //IRegionManager
            _regionManager = regionManager;

            //カテゴリのコンボボックス
            CategoryModelsList = SingleCategoryList.Instance;

            //新規追加
            AddCommand = new DelegateCommand(ExecuteAdd, CanExecuteAdd).ObservesProperty(() => IsEnabled1);

            //修正保存
            UpdateCommand = new DelegateCommand(ExecuteUpdate, CanExecuteUpdate).ObservesProperty(() => IsEnabled2);

            //削除
            DelCommand = new DelegateCommand(ExecuteDel, CanExecuteUpdate).ObservesProperty(() => IsEnabled2);

            //メッセージボックス用
            NotificationRequest = new InteractionRequest<INotification>();

            //確認画面
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
        }

        //新規追加
        private void ExecuteAdd()
        {

            if (CategoryItem == null)
            {
                NotificationRequest.Raise(new Notification { Content = "カテゴリを選択してください", Title = "失敗" });
                return;
            }

            //オブジェクト生成
            QuestionModel obj = new QuestionModel()
            {
                Category = CategoryItem,
                Content = Content,
                Correct = Correct,
                Incorrect1 = Incorrect1,
                Incorrect2 = Incorrect2,
                Incorrect3 = Incorrect3,
                Comment = Comment
            };

            //データベース保存
            if (SQL.AddQuestion(obj))
            {
                NotificationRequest.Raise(new Notification { Content = "新規追加しました", Title = "成功" });
                _regionManager.Regions["ContentRegion1"].RemoveAll();
                _regionManager.RequestNavigate("ContentRegion1", "EditMenuView");
            }
            else
            {
                NotificationRequest.Raise(new Notification { Content = "新規追加に失敗しました", Title = "失敗" });
            }
        }

        private bool CanExecuteAdd()
        {
            return IsEnabled1;
        }

        //編集
        private void ExecuteUpdate()
        {
            //オブジェクト生成
            QuestionModel obj = new QuestionModel()
            {
                Id = Id,
                Category = CategoryItem,
                Content = Content,
                Correct = Correct,
                Incorrect1 = Incorrect1,
                Incorrect2 = Incorrect2,
                Incorrect3 = Incorrect3,
                Comment = Comment
            };

            //データベース保存
            if (SQL.UpdateQuestion(obj))
            {
                NotificationRequest.Raise(new Notification { Content = "編集しました", Title = "成功" });
                _regionManager.Regions["ContentRegion1"].RemoveAll();
                _regionManager.RequestNavigate("ContentRegion1", "EditMenuView");
            }
            else
            {
                NotificationRequest.Raise(new Notification { Content = "編集に失敗しました", Title = "失敗" });
            }

        }

        //削除
        private void ExecuteDel()
        {
            ConfirmationRequest.Raise(
                new Confirmation()
                {
                    Title = "確認",
                    Content = "削除しますか？"
                },
                r =>
                {
                    if (r.Confirmed)
                    {
                        if (SQL.DeleteQuestion(Id))
                        {
                            NotificationRequest.Raise(new Notification { Content = "削除しました", Title = "成功" });
                            _regionManager.Regions["ContentRegion1"].RemoveAll();
                            _regionManager.RequestNavigate("ContentRegion1", "EditMenuView");
                            _regionManager.Regions["ContentRegion2"].RemoveAll();
                            _regionManager.RequestNavigate("ContentRegion2", "EditFormView");
                        }
                        else
                        {
                            NotificationRequest.Raise(new Notification { Content = "削除に失敗しました", Title = "失敗" });
                        }
                    }
                }
            );
        }

        private bool CanExecuteUpdate()
        {
            return IsEnabled2;
        }

        //INavigationAware用メソッド
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

            QuestionModel questionModel = (QuestionModel)navigationContext.Parameters["parameter"];

            if (questionModel == null)
            {
                Title = "新規入力";
                IsEnabled1 = true;
                IsEnabled2 = false;
            }
            else
            {
                Title = "編集";
                SelectedItem = questionModel;
                Id = SelectedItem.Id;
                CategoryItem = SelectedItem.Category;
                Content = SelectedItem.Content;
                Correct = SelectedItem.Correct;
                Incorrect1 = SelectedItem.Incorrect1;
                Incorrect2 = SelectedItem.Incorrect2;
                Incorrect3 = SelectedItem.Incorrect3;
                Comment = SelectedItem.Comment;

                IsEnabled1 = false;
                IsEnabled2 = true;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
