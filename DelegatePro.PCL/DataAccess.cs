using System;
using System.Linq;
using SQLite;
using SQLite.Net;
using SQLite.Net.Interop;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DelegatePro.PCL
{
    public class DataAccess
    {
        protected static DataAccess _instance;
        protected static readonly object padLock = new object();

        private SQLiteConnection _connection;
		private string _databasePath;

        public string DatabasePath
        {
			get { return _databasePath; }
        }

		public static DataAccess Instance
		{
			get
			{
				if (_instance == null)
					throw new Exception ("DataAccess has not been initialized on this platform");

				return _instance;
			}
		}

		public DataAccess (){}

		public static DataAccess GetInstance(ISQLitePlatform platform, string dbDirectory, string userName)
		{
			if (_instance == null)
			{
                _instance = new DataAccess (platform, dbDirectory, userName);
			}

			return _instance;
		}

		private DataAccess (ISQLitePlatform platform, string dbDirectory, string userName)
		{
            _databasePath = Path.Combine (dbDirectory, $"{userName}.db3");

			_connection = new SQLiteConnection (platform, _databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
			CreateTables ();
		}

        public void CloseConnection()
        {
            try
            {
				_connection.Close();
            }
            catch (Exception)
            {
                // do nothing
            }
        }     

        public void CreateTables()
        {
            lock(padLock)
            {
                _connection.CreateTable<Case>();
                _connection.CreateTable<POI>();
                _connection.CreateTable<CasePOI>();
                _connection.CreateTable<GrievanceStatus>();
                _connection.CreateTable<Note>();

                if (_connection.Table<GrievanceStatus>().Count() == 0)
                {
                    _connection.Insert(new GrievanceStatus { ID = 1, Name = "Upheld" });
                    _connection.Insert(new GrievanceStatus { ID = 2, Name = "Declined" });
                    _connection.Insert(new GrievanceStatus { ID = 3, Name = "Closed" });
                }
            }
        }

        public void ExecuteQuery(string query, params object[] param)
        {
            _connection.Execute(query, param);
        }

        public List<T> ExecuteQuery<T>(string query, params object[] param) where T : DataItem, new()
        {
            return _connection.Query<T>(query, param);
        }

        public void SaveItem<T>(T itemToSave) where T : DataItem, new()
		{
			var existingItem = _connection.Table<T>().Where(t => t.ID == itemToSave.ID).FirstOrDefault();
			if (existingItem == null)
			{
				// insert
				_connection.Insert(itemToSave);
			}
			else
			{
				// update
				_connection.Update(itemToSave);
			}
		}

        public void SaveItems<T>(List<T> itemsToSave) where T : DataItem, new()
        {
            var update = new List<T>();
            var insert = new List<T>();

            foreach(var item in itemsToSave)
            {
                var existingItem = _connection.Table<T>().Where(t => t.ID == item.ID).FirstOrDefault();
                if (existingItem == null)
                    insert.Add(item);
                else
                    update.Add(item);
            }

            InsertItems(insert);
            UpdateItems(update);
        }

        public void SaveItemsInt<T>(List<T> itemsToSave) where T : DataItemInt, new()
        {
            var update = new List<T>();
            var insert = new List<T>();

            foreach (var item in itemsToSave)
            {
                var existingItem = _connection.Table<T>().Where(t => t.ID == item.ID).FirstOrDefault();
                if (existingItem == null)
                    insert.Add(item);
                else
                    update.Add(item);
            }

            InsertItems(insert);
            UpdateItems(update);
        }

        public List<T> GetItems<T>() where T : DataItem, new()
		{
			return _connection.Table<T>().ToList();
		}

        public List<T> GetItemsInt<T>() where T : DataItemInt, new()
        {
            return _connection.Table<T>().ToList();
        }

        public List<T> GetItems<T>(Expression<Func<T, bool>> predicate) where T : DataItem, new()
        {
            return _connection.Table<T>().Where(predicate).ToList();
        }

		public void InsertItems<T>(IEnumerable<T> itemsToAdd)
		{
			_connection.InsertAll(itemsToAdd);
		}

        public void UpdateItems<T>(IEnumerable<T> itemsToUpdate)
        {
            _connection.UpdateAll(itemsToUpdate);
        }

		public void DeleteTable<T>()
		{
			_connection.DeleteAll<T>();
		}

		public void DeleteItem<T>(T itemToDelete) where T : DataItem
		{
			_connection.Delete(itemToDelete);
		}

        public void DeleteItem<T>(int primaryKey)
        {
            _connection.Delete<T>(primaryKey);
        }

        public void DeleteItems<T>(IEnumerable<T> itemsToDelete)
        {
            _connection.BeginTransaction();
            foreach(var item in itemsToDelete)
            {
                _connection.Delete(item);
            }
            _connection.Commit();
        }

        public T GetItem<T>(Guid id) where T : DataItem, new()
        {
            return _connection.Table<T>().Where(t => t.ID == id).FirstOrDefault();
        }

        public T GetItemInt<T>(int id) where T : DataItemInt, new()
        {
            return _connection.Table<T>().Where(t => t.ID == id).FirstOrDefault();
        }

        public int GetItemCount<T>(Expression<Func<T, bool>> predicate) where T : DataItem, new()
        {
            return _connection.Table<T>().Where(predicate).Count();
        }
    }
}