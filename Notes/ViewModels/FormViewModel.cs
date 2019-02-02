using Notes.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Notes.ViewModels
{
    public class FormViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {

        //Viewのインスタンスを破棄はfalse
        public bool KeepAlive { get; private set; } = false;

        private readonly IRegionManager _regionManager;

        //NavigationParameters
        //0なら新規入力　1以上は閲覧と編集削除
        private int _naviParam;
        public int NaviParam
        {
            get { return _naviParam; }
            set { SetProperty(ref _naviParam, value); }
        }

        //TextBlock
        private string _textBlockTitle;
        public string TextBlockTitle
        {
            get { return _textBlockTitle; }
            set { SetProperty(ref _textBlockTitle, value); }
        }

        //コンボボックス用
        private ObservableCollection<CategoryModel> _categoryModelsList;
        public ObservableCollection<CategoryModel> CategoryModelsList
        {
            get { return _categoryModelsList; }
            set { SetProperty(ref _categoryModelsList, value); }
        }

        //カテゴリ
        private CategoryModel _category;
        public CategoryModel Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        //memoテーブルのレコードのプロパティ
        private MemoModel _memo;
        public MemoModel Memo
        {
            get { return _memo; }
            set { SetProperty(ref _memo, value); }
        }

        //タイトル
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        //詳細
        private string _detail;
        public string Detail
        {
            get { return _detail; }
            set { SetProperty(ref _detail, value); }
        }

        //新規入力モード
        private bool _isInsert;
        public bool IsInsert
        {
            get { return _isInsert; }
            set { SetProperty(ref _isInsert, value); }
        }

        //編集削除モード
        private bool _isEdit;
        public bool IsEdit
        {
            get { return _isEdit; }
            set { SetProperty(ref _isEdit, value); }
        }

        //検索文字
        private string _searchText2;
        public string SearchText2
        {
            get { return _searchText2; }
            set
            {
                SetProperty(ref _searchText2, value);
                SearchCommand2.RaiseCanExecuteChanged();
                SearchCommand3.RaiseCanExecuteChanged();
            }
        }

        //検索モード
        private bool _isSearch;
        public bool IsSearch
        {
            get { return _isSearch; }
            set { SetProperty(ref _isSearch, value); }
        }

        //削除
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        public DelegateCommand DeleteCommand { get; private set; }
        //編集
        public DelegateCommand UpdateCommand { get; private set; }
        //新規入力
        public DelegateCommand InsertCommand { get; private set; }
        //前検索
        public DelegateCommand<object> SearchCommand2 { get; private set; }
        //後検索
        public DelegateCommand<object> SearchCommand3 { get; private set; }

        //メッセージボックス用
        //25-NotificationRequest
        public InteractionRequest<INotification> NotificationRequest { get; set; }


        //コンストラクタ
        public FormViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            CategoryModelsList = SingleCategoryList.Instance;

            ConfirmationRequest = new InteractionRequest<IConfirmation>();
            DeleteCommand = new DelegateCommand(DeleteExecute, CanExecute1).ObservesCanExecute(() => IsEdit);
            UpdateCommand = new DelegateCommand(UpdateExecute, CanExecute1).ObservesCanExecute(() => IsEdit);
            InsertCommand = new DelegateCommand(InsertExecute, CanExecute0).ObservesCanExecute(() => IsInsert);
            SearchCommand2 = new DelegateCommand<object>(SearchForward, CanExecuteSearch);
            SearchCommand3 = new DelegateCommand<object>(SearchBackward, CanExecuteSearch);

            NotificationRequest = new InteractionRequest<INotification>();

        }


        //削除処理
        private void DeleteExecute()
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
                            if (SQL.DeleteMemo(NaviParam))
                            {
                                NotificationRequest.Raise(new Notification { Content = "削除しました", Title = "成功" });
                                //消す処理
                                Category = null;
                                Title = "";
                                Detail = "";

                                _regionManager.Regions["ContentRegion1"].RemoveAll();
                                _regionManager.RequestNavigate("ContentRegion1", "NotesMainView");
                            }
                            else
                            {
                                NotificationRequest.Raise(new Notification { Content = "削除に失敗しました", Title = "失敗" });
                            }
                        }
                    }
                );
        }

        //編集処理
        private void UpdateExecute()
        {
            if (Category != null)
            {
                if (SQL.UpdateMemo(NaviParam, Category.Id, Title, Detail))
                {
                    NotificationRequest.Raise(new Notification { Content = "更新しました", Title = "成功" });

                    _regionManager.Regions["ContentRegion1"].RemoveAll();
                    _regionManager.RequestNavigate("ContentRegion1", "NotesMainView");
                }
                else
                {
                    NotificationRequest.Raise(new Notification { Content = "更新に失敗しました", Title = "失敗" });
                }
            }
            else
            {
                NotificationRequest.Raise(new Notification { Content = "カテゴリを選択してください", Title = "失敗" });
            }
        }

        //削除・編集可能フラグ
        private bool CanExecute1()
        {
            return IsEdit;
        }

        //新規追加処理
        private void InsertExecute()
        {
            if (Category != null)
            {
                if (SQL.InsertMemo(Category.Id, Title, Detail))
                {
                    NotificationRequest.Raise(new Notification { Content = "新規追加しました", Title = "成功" });
                    //消す処理
                    Category = null;
                    Title = "";
                    Detail = "";

                    _regionManager.Regions["ContentRegion1"].RemoveAll();
                    _regionManager.RequestNavigate("ContentRegion1", "NotesMainView");
                }
                else
                {
                    NotificationRequest.Raise(new Notification { Content = "新規追加に失敗しました", Title = "失敗" });
                }
            }
            else
            {
                NotificationRequest.Raise(new Notification { Content = "カテゴリを選択してください", Title = "失敗" });
            }
        }

        //新規追加可能フラグ
        private bool CanExecute0()
        {
            return IsEdit;
        }


        /*
         * 検索設定はここを参照
         * https://9cubed.info/article/nine_cubed_memo/20180524/
         */

        //前から後ろに向けての検索
        private void SearchForward(object o)
        {
            System.Windows.Controls.TextBox textDetail = (System.Windows.Controls.TextBox)o;

            if (string.IsNullOrEmpty(textDetail.Text))
            {
                return;
            }

            //フォーカス設定
            textDetail.Focus();

            //検索する
            //選択文字列が検索文字列と同じ場合は、次の文字列から検索します
            int offset = textDetail.SelectedText.Equals(SearchText2, StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            int index = textDetail.Text.IndexOf(SearchText2, textDetail.SelectionStart + offset, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                //見つかった場合
                textDetail.Select(index, SearchText2.Length);
            }
        }

        //後ろから前に向けての検索
        private void SearchBackward(object o)
        {
            System.Windows.Controls.TextBox textDetail = (System.Windows.Controls.TextBox)o;

            if (string.IsNullOrEmpty(textDetail.Text))
            {
                return;
            }

            //フォーカス設定
            textDetail.Focus();

            //検索開始位置の設定
            int searchStartIndex = (textDetail.SelectionStart + SearchText2.Length - 1) - 1;
            if (searchStartIndex < 0) return; //先頭の場合は処理を抜けます

            //検索開始位置が末尾以降になる場合は、検索開始位置を末尾にします
            if (searchStartIndex > textDetail.Text.Length) searchStartIndex = textDetail.Text.Length;

            //検索
            int index = textDetail.Text.LastIndexOf(SearchText2, searchStartIndex, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                //見つかった場合
                textDetail.Select(index, SearchText2.Length);
            }
        }

        //検索可能フラグ
        private bool CanExecuteSearch(object arg)
        {
            if (string.IsNullOrEmpty(SearchText2))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //INavigationAwareの3つのメソッド実装

        //画面に遷移してきたとき
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //NaviParamを見て振り分け
            NaviParam = Convert.ToInt32(navigationContext.Parameters["FormViewId"]);

            //新規追加
            if (NaviParam == 0)
            {
                IsInsert = true;
                IsEdit = false;
                TextBlockTitle = "メモの新規追加";
            }
            //編集できる画面
            else
            {
                IsInsert = false;
                IsEdit = true;

                Memo = SQL.EditMemo(NaviParam);

                //LinqでCategoryListのSelectedValueを選ぶ
                Category = CategoryModelsList.Where(x => x.Id == Memo.CategoryId).FirstOrDefault();

                Title = Memo.Title;
                Detail = Memo.Detail;
                TextBlockTitle = Title;
            }
        }

        //引数で渡されたコンテキストのターゲットとなる画面かどうか
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        //画面から離れるとき
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
