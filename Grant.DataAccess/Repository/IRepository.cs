using System.Data.Entity.Infrastructure;

namespace Grant.DataAccess
{
    using System.Linq;
    using System.Threading.Tasks;
    using Grant.Core;
    using Grant.Core.Entities.Base;

    public interface IRepository <T> where T : BaseEntity, new()
    {
        IQueryable<T> GetAll();

        Task<DataResult> Update(T entity, bool deferred = false);

        Task<DataResult> Create(T entity, bool deferred = false);

        Task<DataResult> Delete(long id, bool deferred = false);

        Task<DataResult> SaveChangesAsync();

        DbEntityEntry<T> Entry(T entity);
    }
}
