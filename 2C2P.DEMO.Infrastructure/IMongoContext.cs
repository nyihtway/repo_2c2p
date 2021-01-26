using _2C2P.DEMO.Domain.Models;
using MongoDB.Driver;

namespace _2C2P.DEMO.Infrastructure
{
    public interface IMongoContext
    {
        Collation GetCaseInsensitiveCollation { get; }

        IMongoCollection<Transaction> Transactions { get; }

    }
}
