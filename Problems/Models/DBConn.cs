using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problems.Models
{
    /// <summary>
    /// SQLiteConnection保持のシングルトン
    /// </summary>
    public class DBConn
    {
        //このクラス特有のプロパティ
        public SQLiteConnection Conn { get; private set; }

        //シングルトンのプロパティ
        private static DBConn _instance;
        public static DBConn Instance
        {
            //null合体演算子
            get { return _instance ?? (_instance = new DBConn()); }
        }

        //プライベートコンストラクタ
        private DBConn()
        {
            StreamReader reader = null;
            string dbfile = "";
            //コンパイル後でもパスを変えられるようにするため
            //DBファイルの場所を書いたテキストファイルを読み込む
            try
            {
                using (reader = new StreamReader("ProblemsDB.txt", Encoding.UTF8))
                {
                    dbfile = reader.ReadLine();
                    dbfile = dbfile.Replace("\r", "").Replace("\n", "");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Connに代入
            Conn = new SQLiteConnection("Data Source=" + dbfile);

            //もしもSQLiteのDBファイルがなければテーブルを作成
            if (File.Exists(dbfile) == false)
            {
                CreateTable();
            }
        }

        //もしもSQLiteのDBファイルがなければテーブルを作成
        private void CreateTable()
        {

            Conn.Open();

            SQLiteCommand cmd = Conn.CreateCommand();

            //テーブル作成
            cmd.CommandText = @"
CREATE TABLE `category` (
	`id`	INTEGER,
	`name`	TEXT,
	PRIMARY KEY(`id`)
);
";
            cmd.ExecuteNonQuery();

            //テーブル作成
            cmd.CommandText = @"
CREATE TABLE `question` (
	`id`	INTEGER,
	`category_id`	INTEGER,
	`content`	TEXT,
	`correct`	TEXT,
	`incorrect1`	TEXT,
	`incorrect2`	TEXT,
	`incorrect3`	TEXT,
	`comment`	TEXT,
	PRIMARY KEY(`id`)
);
";
            cmd.ExecuteNonQuery();

            Conn.Close();
        }
    }
}

