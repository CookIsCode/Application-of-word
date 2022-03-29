using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application_of_word
{
    //单词相关主要信息 主数据库抽象
    public class Word
    {
        public int ID { get; set; }
        public bool IsGold { get; set; }
        //用于存储数据库单词主要信息
        public ObservableCollection<string> WordData { get; set; }

        public Word(SqliteDataReader _Reader,int _id,int _count)
        {
            WordData = new ObservableCollection<string>();
            IsGold = false;
            LoadData(_Reader,_id,_count);
        }

        private void LoadData(SqliteDataReader _Reader,int _id,int _count)
        {
            ID = _id;
            for (int i = 0; i < _count; i++)
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
