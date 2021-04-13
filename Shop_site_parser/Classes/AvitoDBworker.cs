using System;
using System.Collections.Generic;
using SQLite;
using Shop_site_parser.Interfaces;
using Shop_site_parser.Model;
using System.Linq;

namespace Shop_site_parser.Classes
{
    class AvitoDBworker: IDBworker, IAvitoDB
    {
        private string data_source = "";
        private SQLiteConnection database;
        static object locker = new object();

        public AvitoDBworker(string _data_source)
        {
            try
            {

                data_source = _data_source;
                database = new SQLiteConnection("TestShopDB.db");

                lock (locker)
                {
                    // проверяем наличие нужной таблицы в БД, если нет - создаем
                    if (database.ExecuteScalar<string>("SELECT name FROM sqlite_master WHERE type='table' AND name='Avito';") == null)
                        database.CreateTable<AvitoDBModel>();
                    else
                        Console.WriteLine("Connect to database");
                }
            }
            catch
            {
                throw new Exception("Can't pDB init");
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
                //Список содержимого таблицы
                return database.Table<AvitoDBModel>().ToList();
            }
        }

        public int RemoveItem(int _id)
        {
            lock (locker)
            {
                //Удаление по id
                return database.Delete<AvitoDBModel>(_id);
            }
        }

        public AvitoDBModel GetByItemID(int _product_id)
        {
            lock(locker)
            {
                return database.Find<AvitoDBModel>(it => it.product_id == _product_id);
            }
        }

        public int ResetActualState()
        {
            lock (locker)
            {
                var items = database.Table<AvitoDBModel>().ToList();
                foreach (var it in items)
                {
                    it.actual = false;
                }
                return database.UpdateAll(items);
            }
        }

        public int UpdateItem(AvitoDBModel _updateItem)
        {
            lock (locker)
            {
                return database.Update(_updateItem);
            }
        }

        public void ClearNonActual()
        {
            lock(locker)
            {
                var items = database.Table<AvitoDBModel>().ToList().Where(it => it.actual == false);
                foreach (var item in items)
                {
                    database.Delete(item);
                }
            }
        }

        public void CloseDBConnection()
        {
            lock (locker)
            {
                //Закрыть соединение
                database.Close();
            }
        }
    }
}
