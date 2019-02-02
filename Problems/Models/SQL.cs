using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problems.Models
{
    public class SQL
    {
        /// <summary>
        /// カテゴリコレクションを返す
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<CategoryModel> CategoryModels()
        {

            ObservableCollection<CategoryModel> list = new ObservableCollection<CategoryModel>();

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM category ORDER BY id;";

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new CategoryModel()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString()
                });
            }

            reader.Close();

            Conn.Close();

            return list;
        }

        /// <summary>
        /// 問題のコレクションを返す
        /// 引数のコレクションはCategoryModel型にするために使う
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<QuestionModel> QuestionModels(ObservableCollection<CategoryModel> categories)
        {

            ObservableCollection<QuestionModel> list = new ObservableCollection<QuestionModel>();

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM question ORDER BY id DESC;";

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new QuestionModel()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    //Single()が大事
                    Category = categories.Where(x => x.Id == Convert.ToInt32(reader["category_id"])).Single(),
                    Content = reader["content"].ToString(),
                    Correct = reader["correct"].ToString(),
                    Incorrect1 = reader["incorrect1"].ToString(),
                    Incorrect2 = reader["incorrect2"].ToString(),
                    Incorrect3 = reader["incorrect3"].ToString(),
                    Comment = reader["comment"].ToString()
                });
            }

            reader.Close();

            Conn.Close();

            return list;
        }

        /// <summary>
        /// カテゴリ保存
        /// </summary>
        /// <param name="Categories"></param>
        /// <returns></returns>
        public static bool CategorySave(ObservableCollection<CategoryModel> Categories)
        {

            bool ok = true;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteTransaction transaction = Conn.BeginTransaction();

            SQLiteCommand cmd = Conn.CreateCommand();

            try
            {
                //一度全部消してから
                cmd.CommandText = @"DELETE FROM category;";
                cmd.ExecuteNonQuery();

                //新たにINSERT
                foreach (var v in Categories)
                {
                    cmd.CommandText = @"INSERT INTO category(id, name) values (@ID, @NAME);";
                    cmd.Parameters.Add(new SQLiteParameter("@ID", v.Id));
                    cmd.Parameters.Add(new SQLiteParameter("@NAME", v.Name));
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();

                ok = false;

                Console.WriteLine(e.ToString());
            }
            finally
            {
                Conn.Close();
            }

            return ok;
        }

        /// <summary>
        /// 問題新規追加
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool AddQuestion(QuestionModel obj)
        {
            bool ok = true;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteTransaction transaction = Conn.BeginTransaction();

            SQLiteCommand cmd = Conn.CreateCommand();

            try
            {
                //新規追加
                cmd.CommandText = @"
INSERT INTO question(category_id, content, correct, incorrect1, incorrect2, incorrect3, comment)
values (@CATEGORY_ID, @CONTENT, @CORRECT, @INCORRECT1, @INCORRECT2, @INCORRECT3, @COMMENT);
                ";
                cmd.Parameters.Add(new SQLiteParameter("@CATEGORY_ID", obj.Category.Id));
                cmd.Parameters.Add(new SQLiteParameter("@CONTENT", obj.Content));
                cmd.Parameters.Add(new SQLiteParameter("@CORRECT", obj.Correct));
                cmd.Parameters.Add(new SQLiteParameter("@INCORRECT1", obj.Incorrect1));
                cmd.Parameters.Add(new SQLiteParameter("@INCORRECT2", obj.Incorrect2));
                cmd.Parameters.Add(new SQLiteParameter("@INCORRECT3", obj.Incorrect3));
                cmd.Parameters.Add(new SQLiteParameter("@COMMENT", obj.Comment));
                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();

                ok = false;

                Console.WriteLine(e.ToString());
            }
            finally
            {
                Conn.Close();
            }

            return ok;

        }

        /// <summary>
        /// 問題修正
        /// </summary>
        /// <returns></returns>
        public static bool UpdateQuestion(QuestionModel obj)
        {

            bool ok = true;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteTransaction transaction = Conn.BeginTransaction();

            SQLiteCommand cmd = Conn.CreateCommand();

            try
            {
                //更新
                cmd.CommandText = @"
UPDATE question SET
category_id=@CATEGORY_ID,
content=@CONTENT,
correct=@CORRECT,
incorrect1=@INCORRECT1,
incorrect2=@INCORRECT2,
incorrect3=@INCORRECT3,
comment=@COMMENT
WHERE id= @ID;
                ";
                cmd.Parameters.Add(new SQLiteParameter("@CATEGORY_ID", obj.Category.Id));
                cmd.Parameters.Add(new SQLiteParameter("@CONTENT", obj.Content));
                cmd.Parameters.Add(new SQLiteParameter("@CORRECT", obj.Correct));
                cmd.Parameters.Add(new SQLiteParameter("@INCORRECT1", obj.Incorrect1));
                cmd.Parameters.Add(new SQLiteParameter("@INCORRECT2", obj.Incorrect2));
                cmd.Parameters.Add(new SQLiteParameter("@INCORRECT3", obj.Incorrect3));
                cmd.Parameters.Add(new SQLiteParameter("@COMMENT", obj.Comment));
                cmd.Parameters.Add(new SQLiteParameter("@ID", obj.Id));
                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();

                ok = false;

                Console.WriteLine(e.ToString());
            }
            finally
            {
                Conn.Close();
            }

            return ok;
        }

        /// <summary>
        /// 問題削除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool DeleteQuestion(int id)
        {
            bool ok = true;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteTransaction transaction = Conn.BeginTransaction();

            SQLiteCommand cmd = Conn.CreateCommand();

            try
            {
                //削除
                cmd.CommandText = @"DELETE FROM question WHERE id= @ID;";
                cmd.Parameters.Add(new SQLiteParameter("@ID", id));
                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();

                ok = false;

                Console.WriteLine(e.ToString());
            }
            finally
            {
                Conn.Close();
            }

            return ok;
        }


        /// <summary>
        /// 問題検索
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static ObservableCollection<QuestionModel> SearchQuestionModels(ObservableCollection<CategoryModel> categories, string q)
        {

            ObservableCollection<QuestionModel> list = new ObservableCollection<QuestionModel>();

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            cmd.CommandText = @"
SELECT * FROM question WHERE 
content LIKE @Q OR 
correct LIKE @Q OR 
incorrect1 LIKE @Q OR 
incorrect2 LIKE @Q OR 
incorrect3 LIKE @Q OR 
comment LIKE @Q
ORDER BY id DESC;";

            cmd.Parameters.Add(new SQLiteParameter("@Q", "%" + q + "%"));

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new QuestionModel()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    //Single()が大事
                    Category = categories.Where(x => x.Id == Convert.ToInt32(reader["category_id"])).Single(),
                    Content = reader["content"].ToString(),
                    Correct = reader["correct"].ToString(),
                    Incorrect1 = reader["incorrect1"].ToString(),
                    Incorrect2 = reader["incorrect2"].ToString(),
                    Incorrect3 = reader["incorrect3"].ToString(),
                    Comment = reader["comment"].ToString()
                });
            }

            reader.Close();

            Conn.Close();

            return list;
        }


        /// <summary>
        /// 問題抽出
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static List<QuestionModel> SelectQuestions(List<CategoryModel> categories)
        {
            //返却するリストの初期化
            List<QuestionModel> questions = new List<QuestionModel>();

            //カテゴリ抽出のためのIN句用文字列
            string inku = "";

            //IN句作成
            if (categories != null)
            {
                //IN句で使う文字列を取得　WHERE カラム IN(値1, 値2, ...)
                //例 "1, 2"
                //SQLiteParameterではうまく動かない
                var query = categories.Select(x => x.Id);
                inku = string.Join(", ", query);
            }

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            if (inku.Equals(""))
            {
                cmd.CommandText = @"
SELECT t1.id, t1.category_id, t2.name, t1.content, t1.correct, t1.incorrect1, t1.incorrect2, t1.incorrect3, t1.comment
FROM question AS t1 LEFT JOIN category AS t2 ON t1.category_id = t2.id;
                ";
            }
            else
            {
                cmd.CommandText = $@"SELECT t1.id, t1.category_id, t2.name, t1.content, t1.correct, t1.incorrect1, t1.incorrect2, t1.incorrect3, t1.comment
FROM question AS t1 LEFT JOIN category AS t2 ON t1.category_id = t2.id WHERE t1.category_id IN({inku});";

                //                cmd.CommandText = @"
                //SELECT t1.id, t1.category_id, t2.name, t1.content, t1.correct, t1.incorrect1, t1.incorrect2, t1.incorrect3, t1.comment
                //FROM question AS t1 LEFT JOIN category AS t2 ON t1.category_id = t2.id WHERE t1.category_id IN(@IN);
                //                ";
                //これは２つ以上入ると動かない。SQLインジェクション対策なので、一つの値として扱われているのかも？
                //                cmd.Parameters.Add(new SQLiteParameter("@IN", inku));

            }

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                questions.Add(
                    new QuestionModel()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Category = new CategoryModel()
                        {
                            Id = Convert.ToInt32(reader["category_id"]),
                            Name = reader["name"].ToString(),
                        },
                        Content = reader["content"].ToString(),
                        Correct = reader["correct"].ToString(),
                        Incorrect1 = reader["incorrect1"].ToString(),
                        Incorrect2 = reader["incorrect2"].ToString(),
                        Incorrect3 = reader["incorrect3"].ToString(),
                        Comment = reader["comment"].ToString()
                    }
                );
            }

            reader.Close();

            Conn.Close();

            return questions;

        }
    }
}
