using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application_of_word
{
    //单词相关主要信息
    public class Word
    {
        public int ID { get; set; }
        public int WordID { get; set; }

        //用于存储数据库单词主要信息
        public ObservableCollection<string> WordData { get; set; }

        public Word(SqliteDataReader _Reader)
        {
            WordData = new ObservableCollection<string>();
            LoadData(_Reader);
        }
        private void LoadData(SqliteDataReader _Reader)
        {
            ID = _Reader.GetInt32(0);
            WordID = _Reader.GetInt32(2);
            for (int i = 3; i <= 16; i++)
            {
                if (_Reader.IsDBNull(i))
                {
                    WordData.Add("");
                }
                else
                {
                    WordData.Add(_Reader.GetString(i));
                }
            }
        }
    }

}
