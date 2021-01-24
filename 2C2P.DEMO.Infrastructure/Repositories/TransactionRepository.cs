using _2C2P.DEMO.Domain.Models;
using _2C2P.DEMO.Infrastructure.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IMongoContext _context;
        private readonly FilterDefinitionBuilder<Transaction> _filterBuilder;

        public TransactionRepository(IMongoContext context)
        {
            _context = context;
            _filterBuilder = Builders<Transaction>.Filter;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByCurrency(string currency)
        {
            var filter = _filterBuilder.Eq(x => x.CurrencyCode, currency);
            var options = new FindOptions<Transaction>() { Collation = _context.GetCaseInsensitiveCollation };
            var result = await _context.Transactions.FindAsync(filter, options);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStartDateEndDate(DateTime startDate, DateTime endDate)
        {
            var filter = _filterBuilder.Gte(x => x.TransactionDate, startDate) & _filterBuilder.Lte(x => x.TransactionDate, endDate);
            var options = new FindOptions<Transaction>() { Collation = _context.GetCaseInsensitiveCollation };
            var result = await _context.Transactions.FindAsync(filter, options);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStatus(string status)
        {
            var filter = _filterBuilder.Eq(x => x.Status, status);
            var options = new FindOptions<Transaction>() { Collation = _context.GetCaseInsensitiveCollation };
            var result = await _context.Transactions.FindAsync(filter, options);

            return await result.ToListAsync();
        }

        public async Task Insert(Transaction document)
        {
            await _context.Transactions.InsertOneAsync(document);
        }

        public Task InsertMany(List<Transaction> documents)
        {
            var options = new InsertManyOptions() { BypassDocumentValidation = true, IsOrdered = false };

            _context.Transactions.InsertMany(documents, options);
            return Task.CompletedTask;
        }


    }
}
