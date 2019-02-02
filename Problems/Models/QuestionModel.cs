using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problems.Models
{
    public class QuestionModel : BindableBase
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private CategoryModel _category;
        public CategoryModel Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private string _correct;
        public string Correct
        {
            get { return _correct; }
            set { SetProperty(ref _correct, value); }
        }

        private string _incorrect1;
        public string Incorrect1
        {
            get { return _incorrect1; }
            set { SetProperty(ref _incorrect1, value); }
        }

        private string _incorrect2;
        public string Incorrect2
        {
            get { return _incorrect2; }
            set { SetProperty(ref _incorrect2, value); }
        }

        private string _incorrect3;
        public string Incorrect3
        {
            get { return _incorrect3; }
            set { SetProperty(ref _incorrect3, value); }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        //内容がわかるように表示
        public override string ToString()
        {
            //先頭から何文字表示するのか指定
            int num = 50;

            //改行削除
            string str = Content.Replace("\r", "").Replace("\n", "");

            //文字数取得
            int count = str.Length;

            //追加文字
            string add = "…";

            if (num >= count)
            {
                num = count;
                add = "";
            }

            return $"{Id}>>>{Category.Name}>>>{str.Substring(0, num)}{add}";
        }
    }
}
