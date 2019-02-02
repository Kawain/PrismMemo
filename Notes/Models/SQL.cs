using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Models
{
    public class SQL
    {
        /// <summary>
        /// トップページ
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<TopModel> TopModels()
        {

            ObservableCollection<TopModel> list = new ObservableCollection<TopModel>();

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            cmd.CommandText = @"
SELECT t1.id, t2.name, t1.title FROM memo AS t1 LEFT JOIN category AS t2 ON t1.category_id = t2.id ORDER BY t1.id DESC;
";
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new TopModel()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Category = reader["name"].ToString(),
                    Title = reader["title"].ToString()
                });
            }

            reader.Close();

            Conn.Close();

            return list;
        }

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

            cmd.CommandText = @"SELECT * FROM category ORDER BY name;";

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
        /// 個別記事
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MemoModel EditMemo(int id)
        {
            MemoModel memo = null;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM memo WHERE id = @ID;";
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                memo = new MemoModel()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    CategoryId = Convert.ToInt32(reader["category_id"]),
                    Title = reader["title"].ToString(),
                    Detail = reader["detail"].ToString()
                };
            }

            reader.Close();

            Conn.Close();

            return memo;
        }


        /// <summary>
        /// トップページの検索
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static ObservableCollection<TopModel> KeywordSelect(string q)
        {

            ObservableCollection<TopModel> list = new ObservableCollection<TopModel>();

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            cmd.CommandText = @"
SELECT t1.id, t2.name, t1.title FROM memo AS t1 LEFT JOIN category AS t2 ON t1.category_id = t2.id
WHERE t1.title LIKE @Q OR t1.detail LIKE @Q ORDER BY t1.id DESC;

";
            cmd.Parameters.Add(new SQLiteParameter("@Q", "%" + q + "%"));

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(
                    new TopModel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Category = reader["name"].ToString(),
                        Title = reader["title"].ToString()
                    }
                );
            }

            reader.Close();

            Conn.Close();

            return list;
        }

        /// <summary>
        /// トップページのカテゴリ検索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ObservableCollection<TopModel> CategorySelect(int id)
        {

            ObservableCollection<TopModel> list = new ObservableCollection<TopModel>();

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            cmd.CommandText = @"
SELECT t1.id, t2.name, t1.title 
FROM memo AS t1 LEFT JOIN category AS t2 ON
t1.category_id = t2.id WHERE t2.id = @ID ORDER BY t1.id DESC;
";

            cmd.Parameters.Add(new SQLiteParameter("@ID", id));

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(
                    new TopModel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Category = reader["name"].ToString(),
                        Title = reader["title"].ToString()
                    }
                );
            }

            reader.Close();

            Conn.Close();

            return list;
        }

        /// <summary>
        /// メモの新規追加
        /// </summary>
        /// <param name="category_id"></param>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static bool InsertMemo(int category_id, string title, string detail)
        {

            bool ok = true;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteTransaction transaction = Conn.BeginTransaction();

            SQLiteCommand cmd = Conn.CreateCommand();

            try
            {
                //新規追加
                cmd.CommandText = @"INSERT INTO memo(category_id, title, detail) values (@CATEGORY_ID, @TITLE, @DETAIL);";
                cmd.Parameters.Add(new SQLiteParameter("@CATEGORY_ID", category_id));
                cmd.Parameters.Add(new SQLiteParameter("@TITLE", title));
                cmd.Parameters.Add(new SQLiteParameter("@DETAIL", detail));
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
        /// メモの更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category_id"></param>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static bool UpdateMemo(int id, int category_id, string title, string detail)
        {

            bool ok = true;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteTransaction transaction = Conn.BeginTransaction();

            SQLiteCommand cmd = Conn.CreateCommand();

            try
            {
                //更新
                cmd.CommandText = @"UPDATE memo SET category_id=@CATEGORY_ID, title=@TITLE, detail=@DETAIL WHERE id= @ID;";
                cmd.Parameters.Add(new SQLiteParameter("@CATEGORY_ID", category_id));
                cmd.Parameters.Add(new SQLiteParameter("@TITLE", title));
                cmd.Parameters.Add(new SQLiteParameter("@DETAIL", detail));
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
        /// メモの削除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteMemo(int id)
        {

            bool ok = true;

            SQLiteConnection Conn = DBConn.Instance.Conn;

            Conn.Open();

            SQLiteTransaction transaction = Conn.BeginTransaction();

            SQLiteCommand cmd = Conn.CreateCommand();

            try
            {
                //削除
                cmd.CommandText = @"DELETE FROM memo WHERE id= @ID;";
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
    }
}
