using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Shop_site_parser.Interfaces;
using Shop_site_parser.Model;

namespace Shop_site_parser.Classes
{
    class AvitoDBworker: IDBworker, IAvitoDB
    {
        private string data_source = "";
        private SQLiteConnection database;
        static object locker = new object();

        public AvitoDBworker(string _data_source)
        {
            data_source = _data_source;
            database = new SQLiteConnection("TestShopDB.db");

            lock (locker)
            {
                if (database.ExecuteScalar<string>("SELECT name FROM sqlite_master WHERE type='table' AND name='Avito';") == null)
                    database.CreateTable<AvitoDBModel>();
                else
                    Console.WriteLine("Connect to database");
            }
        }

        public int WriteItem(AvitoDBModel _newItem)
        {
            lock(locker)
            {
                return database.Insert(_newItem);
            }
        }

        public List<AvitoDBModel> GetDataList()
        {
            lock (locker)
            {
                return database.Table<AvitoDBModel>().ToList();
            }
        }

        public int RemoveItem(int _id)
        {
            lock (locker)
            {
                return database.Delete<AvitoDBModel>(_id);
            }
        }

        public void CloseDBConnection()
        {
            lock (locker)
            {
                database.Close();
            }
        }
    }
}
