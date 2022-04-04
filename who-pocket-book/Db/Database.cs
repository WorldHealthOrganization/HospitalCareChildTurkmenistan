using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLiteNetExtensionsAsync.Extensions;
using who_pocket_book.Models;

namespace who_pocket_book.Db
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Measure>().Wait();
        }

        public Task<List<Measure>> GetMeasuresAsync() => _database.GetAllWithChildrenAsync<Measure>();

        public Task SaveMeasuresAsync(List<Measure> measures)
        {
            return _database.InsertAllWithChildrenAsync(measures);
        }
    }
}
