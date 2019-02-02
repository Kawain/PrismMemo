using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problems.Models
{
    /// <summary>
    /// https://qiita.com/7of9/items/9067fe160aa110f7b838
    /// シングルトン
    /// 初回起動時にここでDBからObservableCollectionを作成
    /// その後はバインドで使いまわし
    /// </summary>
    public class SingleCategoryList
    {
        private static ObservableCollection<CategoryModel> instance;

        private SingleCategoryList()
        {

        }

        public static ObservableCollection<CategoryModel> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = SQL.CategoryModels();
                }
                return instance;
            }
        }
    }
}
