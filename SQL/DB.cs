using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ChusanExplorer
{
    public static class DBLoader
    {
        public static Action Loaded;

        public static List<Player> users;

        static SQLiteConnection db;
        public static bool Active
        {
            get => db != null;
        }

        public static void Init()
        {
            PackLoader.Loaded += Flush;
        }
        static void clearData()
        {
            users = new List<Player>();
            db = null;
        }
        public static void Flush()
        {
            clearData();
            var connStr = $"Data Source={Config.DirAquaDB};Version=3;";
            try
            {
                db = new SQLiteConnection(connStr);
                db.Open();

                // users
                using (var r = Read("select * from chusan_user_data"))
                {
                    while (r.Read()) users.Add(new Player(r));
                }
            }
            catch (Exception e)
            {
                e.ShowError($"加载数据库出错 ({Config.DirAquaDB})");
                clearData();
            }
            Loaded.Invoke();
        }

        public static SQLiteDataReader Read(string cmd)
        {
            var cursor = db.CreateCommand();
            cursor.CommandText = cmd;
            return cursor.ExecuteReader();
        }

        public static int Write(string cmd)
        {
            var cursor = db.CreateCommand();
            cursor.CommandText = cmd;
            return cursor.ExecuteNonQuery();
        }
    }
}
