using System.Data.Entity.Infrastructure;

namespace Grant.Services.DomainService
{
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities.Base;

    public interface IDomainService<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();

        Task<IQueryable<T>> GetAllAsync();

        IQueryable<T> GetAll(string name);

        Task<T> Get(long id);

        Task<DataResult> Update(T entity, bool deferred = false);

        Task<DataResult> Create(T entity);

        Task<DataResult> Delete(long id);

        Task<bool> ExistsAsync(long id);

        bool Exists(long id);

        DbEntityEntry<T> Entry(T entity);
    }
}
