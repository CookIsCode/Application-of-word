using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;


namespace Application_of_word
{
    internal class Database
    {
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

        SqliteDataReader ReadDatabase(string _sql)
        {
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = _sql;
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        //获取所有词书
        public ObservableCollection<Word> ReadTableData(string _table, string _book)
        {
            string SQL = @"SELECT ID, Book, WordID, Word, USPhoneticSymbol , UKPhoneticSymbol, n, v, vi, vt , adj, adv, prep, conj, int , aux, num FROM AllWord";
            if (_book != "All")
            {
                SQL += " WHERE Book=" + "\"" + _book + "\"";
            }
            var Reader = ReadDatabase(SQL);
            var obj = new ObservableCollection<Word>();
            while (Reader.Read())
            {
                obj.Add(new Word(Reader));
            }
            return obj;
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

        public ObservableCollection<string> FindTables()
        {
            var obj = new ObservableCollection<string>();
            var Reader = ReadDatabase("SELECT name FROM sqlite_master where type='table' order by name");
            while (Reader.Read())
            {
                obj.Add(Reader.GetString(0));
            }
            return obj;
        }
    }
}