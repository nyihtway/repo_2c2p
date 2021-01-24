using _2C2P.DEMO.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Infrastructure.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetTransactionsByCurrency(string currency);
        Task<IEnumerable<Transaction>> GetTransactionsByStartDateEndDate(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Transaction>> GetTransactionsByStatus(string status);
    }
}
