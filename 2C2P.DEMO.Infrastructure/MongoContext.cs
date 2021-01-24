using _2C2P.DEMO.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2C2P.DEMO.Infrastructure
{
    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase _database;
        public Collation GetCaseInsensitiveCollation =>  new Collation("en", strength: CollationStrength.Primary);

        public MongoContext(IMongoClient client, string dbName)
        {
            _database = client.GetDatabase(dbName);
        }

        public IMongoCollection<Transaction> Transactions => _database.GetCollection<Transaction>("interfaceStats");
    }
}
