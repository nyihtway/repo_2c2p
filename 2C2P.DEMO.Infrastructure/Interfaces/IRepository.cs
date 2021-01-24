using System.Collections.Generic;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Infrastructure.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task Insert(TEntity document);
        Task InsertMany(List<TEntity> documents);
    }
}
