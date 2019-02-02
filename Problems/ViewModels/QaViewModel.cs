using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Problems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Problems.ViewModels
{
    public class QaViewModel : BindableBase, IRegionMemberLifetime, INavigationAware
    {

        //Viewのインスタンスを破棄はfalse
        public bool KeepAlive { get; private set; } = false;

        //--------------------------------------
        //第何問
        private string _times;
        public string Times
        {
            get { return _times; }
            set { SetProperty(ref _times, value); }
        }

        //問題文
        private string _question;
        public string Question
        {
            get { return _question; }
            set { SetProperty(ref _question, value); }
        }

        //選択肢1
        private bool _radioChoice1 = false;
        public bool RadioChoice1
        {
            get { return _radioChoice1; }
            set
            {
                SetProperty(ref _radioChoice1, value);
                AnswerCommand.RaiseCanExecuteChanged();
            }
        }

        private string _choice1;
        public string Choice1
        {
            get { return _choice1; }
            set { SetProperty(ref _choice1, value); }
        }

        //選択肢2
        private bool _radioChoice2 = false;
        public bool RadioChoice2
        {
            get { return _radioChoice2; }
            set
            {
                SetProperty(ref _radioChoice2, value);
                AnswerCommand.RaiseCanExecuteChanged();
            }
        }

        private string _choice2;
        public string Choice2
        {
            get { return _choice2; }
            set { SetProperty(ref _choice2, value); }
        }

        //選択肢3
        private bool _radioChoice3 = false;
        public bool RadioChoice3
        {
            get { return _radioChoice3; }
            set
            {
                SetProperty(ref _radioChoice3, value);
                AnswerCommand.RaiseCanExecuteChanged();
            }
        }

        private string _choice3;
        public string Choice3
        {
            get { return _choice3; }
            set { SetProperty(ref _choice3, value); }
        }

        //選択肢4
        private bool _radioChoice4 = false;
        public bool RadioChoice4
        {
            get { return _radioChoice4; }
            set
            {
                SetProperty(ref _radioChoice4, value);
                AnswerCommand.RaiseCanExecuteChanged();
            }
        }

        private string _choice4;
        public string Choice4
        {
            get { return _choice4; }
            set { SetProperty(ref _choice4, value); }
        }


        //判定
        private string _checkAnswer;
        public string CheckAnswer
        {
            get { return _checkAnswer; }
            set { SetProperty(ref _checkAnswer, value); }
        }

        //正解
        private string _correctAnswer;
        public string CorrectAnswer
        {
            get { return _correctAnswer; }
            set { SetProperty(ref _correctAnswer, value); }
        }

        //解説
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        //正解率
        private string _rate;
        public string Rate
        {
            get { return _rate; }
            set { SetProperty(ref _rate, value); }
        }


        //--------------------------------------

        //問題集
        private List<QuestionModel> _questionsList;
        public List<QuestionModel> QuestionsList
        {
            get { return _questionsList; }
            set { SetProperty(ref _questionsList, value); }
        }

        //問題集のインデックス
        private int _index = 0;
        public int Index
        {
            get { return _index; }
            set
            {
                SetProperty(ref _index, value);
                NextQuestionCommand.RaiseCanExecuteChanged();
            }
        }

        //問題件数
        private int number = 0;

        //問題
        private QuestionModel q;

        //正解数
        private int correctNumber = 0;

        //問題の選択肢
        private Dictionary<string, bool> _choiceDic;
        public Dictionary<string, bool> ChoiceDic
        {
            get { return _choiceDic; }
            set { SetProperty(ref _choiceDic, value); }
        }

        //StackPanelのVisibility
        private Visibility _stackPanelVisibility = Visibility.Collapsed;
        public Visibility StackPanelVisibility
        {
            get { return _stackPanelVisibility; }
            set { SetProperty(ref _stackPanelVisibility, value); }
        }

        //AnswerButtonのVisibility
        private Visibility _answerButtonVisibility = Visibility.Visible;
        public Visibility AnswerButtonVisibility
        {
            get { return _answerButtonVisibility; }
            set { SetProperty(ref _answerButtonVisibility, value); }
        }

        //答え合わせ
        public DelegateCommand AnswerCommand { get; private set; }

        //次の問題
        public DelegateCommand<object> NextQuestionCommand { get; private set; }

        //コンストラクタ
        public QaViewModel()
        {
            AnswerCommand = new DelegateCommand(ExecuteAnswer, CanExecuteAnswer);
            NextQuestionCommand = new DelegateCommand<object>(ExecuteNext, CanExecuteNext);
        }

        //次の問題実行可否
        private bool CanExecuteNext(object arg)
        {
            if (number - 1 < Index)
            {
                return false;
            }
            return true;
        }

        //次の問題
        private void ExecuteNext(object obj)
        {

            Substitution();

            var sv = (ScrollViewer)obj;

            sv.ScrollToTop();

        }

        //答え合わせボタンの実行可否
        private bool CanExecuteAnswer()
        {
            if (RadioChoice1 || RadioChoice2 || RadioChoice3 || RadioChoice4)
            {
                return true;
            }
            return false;
        }

        //答え合わせ
        private void ExecuteAnswer()
        {
            bool ok = false;

            if (RadioChoice1)
            {
                if (ChoiceDic[Choice1])
                {
                    ok = true;
                }
            }

            if (RadioChoice2)
            {
                if (ChoiceDic[Choice2])
                {
                    ok = true;
                }
            }

            if (RadioChoice3)
            {
                if (ChoiceDic[Choice3])
                {
                    ok = true;
                }
            }

            if (RadioChoice4)
            {
                if (ChoiceDic[Choice4])
                {
                    ok = true;
                }
            }

            if (ok)
            {
                CheckAnswer = "【正解】";
                CorrectAnswer = "";
                correctNumber++;
            }
            else
            {
                CheckAnswer = "【不正解】　正解は↓";
                CorrectAnswer = q.Correct;
            }

            //正解率
            float r = 0;

            if (correctNumber > 0)
            {
                r = correctNumber / (float)Index;
            }

            Rate = $"{Index}問中{correctNumber}問正解\n正解率 {Math.Round(r, 4) * 100}%";


            AnswerButtonVisibility = Visibility.Hidden;
            StackPanelVisibility = Visibility.Visible;

        }


        // 問題をプロパティに代入
        public void Substitution()
        {

            //初期設定
            //Visibility
            AnswerButtonVisibility = Visibility.Visible;
            StackPanelVisibility = Visibility.Collapsed;
            //ラジオボタンをオフ
            RadioChoice1 = false;
            RadioChoice2 = false;
            RadioChoice3 = false;
            RadioChoice4 = false;


            q = QuestionsList[Index];

            Times = $"第{Index + 1}問";
            Question = q.Category.Name + "からの問題\n\n" + q.Content;

            //選択肢のDictionaryを作成
            ChoiceDic = new Dictionary<string, bool>();

            //Dictionaryに追加 正解はtrue
            ChoiceDic[q.Correct] = true;
            ChoiceDic[q.Incorrect1] = false;
            ChoiceDic[q.Incorrect2] = false;
            ChoiceDic[q.Incorrect3] = false;

            //キーの値をシャッフルし、配列にしておく
            string[] keys = ChoiceDic.Keys.OrderBy(key => Guid.NewGuid()).ToArray();

            Choice1 = keys[0];
            Choice2 = keys[1];
            Choice3 = keys[2];
            Choice4 = keys[3];

            Comment = "【解説】\n\n" + q.Comment;

            Index++;

        }

        //ここがコンストラクタみたいなものになる
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

            //引数を受け取り
            List<CategoryModel> categories = (List<CategoryModel>)navigationContext.Parameters["parameter"];

            //SelectQuestionsを取得
            QuestionsList = SQL.SelectQuestions(categories);

            //問題件数代入
            number = QuestionsList.Count;

            Console.WriteLine(number);

            //QuestionsListをシャッフルする
            QuestionsList = QuestionsList.OrderBy(a => Guid.NewGuid()).ToList();

            //第1問目スタート
            Substitution();

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
