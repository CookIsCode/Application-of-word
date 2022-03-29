using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace Application_of_word
{
    internal class Database
    {
        public event EventHandler<EventArgs> AsynQueried;
        private SqliteConnection connection;
        
        public Database(string _DataPath)
        {
            if (File.Exists(_DataPath))
            {
                string Coneectionstr = "Data Source = " + _DataPath;
                connection = new SqliteConnection(Coneectionstr);
                connection.Open();
            }
            else
            {
                System.Windows.MessageBox.Show("Database don't exist");
            }

        }

        ~Database()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }
   
        public void ExecuteSqlCommand(string _sql)
        {
            var command= connection.CreateCommand();
            command.CommandText = _sql;
            command.ExecuteNonQuery();
        }

        public void AsynExecuteSqlCommand(List<string> _SqlList)
        {
            Thread ExecuteCommandThread = new Thread(() => { foreach (string _Sql in _SqlList) { ExecuteSqlCommand(_Sql); } });
            ExecuteCommandThread.Priority = ThreadPriority.BelowNormal;
            ExecuteCommandThread.Start();
        }

        public SqliteDataReader ReadDatabase(string _sql)
        {
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = _sql;
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"数据库查询错误",MessageBoxButton.OK,MessageBoxImage.Error);
                return null;
            }
        }

        //获取所有词书
        public ObservableCollection<Word> ReadInerWordBook(string _table, string _book)
        {
            /*
            string SQL = @"SELECT ID, Book, WordID, Word, USPhoneticSymbol , UKPhoneticSymbol, n, v, vi, vt , adj, adv, prep, conj, int , aux, num FROM AllWord";
            if (_book != "All")
            {
                SQL += " WHERE Book=" + "\"" + _book + "\"";
            }
            var Reader = ReadDatabase(SQL);
            var obj = new ObservableCollection<Word>();
            int count = 1;
            while (Reader.Read())
            {
                obj.Add(new Word(Reader,count++));
            }
            return obj;*/
            return null;
        }

        //用于combox检索词书
        public ObservableCollection<string> FindBooksName(string _table)
        {
           
            var Reader =ReadDatabase("SELECT DISTINCT Book FROM " + _table);
            var obj = new ObservableCollection<string>();
            obj.Add("None"); obj.Add("All");
            while (Reader.Read())
            {
                obj.Add(Reader.GetString(0));
            }
            return obj;
        }
        //访问数据库中表数据通过传入参数来限定是否上锁
        public ObservableCollection<string> FindTables(bool _lock)
        {
            string Temp;
            var obj = new ObservableCollection<string>();
            var Reader = ReadDatabase("SELECT name FROM sqlite_master where type='table' order by name");
            while (Reader.Read())
            {
                Temp= Reader.GetString(0);
                if (_lock && ((Temp == "AllWord") || (Temp == "sqlite_sequence")))
                    continue;
                obj.Add(Temp);
            }
            return obj;
        }
    }

    internal class QueryData
    {
        public SqliteDataReader? Result { get; set; }
        public object? InvokeObject { get; set; }
        public QueryData(SqliteDataReader _result,object _invokeobject)
        {
            Result = _result;
            InvokeObject = _invokeobject;
        }
    }
}