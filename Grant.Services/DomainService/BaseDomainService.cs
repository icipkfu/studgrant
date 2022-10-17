using System.Data.Entity.Infrastructure;

namespace Grant.Services.DomainService
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities.Base;
    using DataAccess;

    public class BaseDomainService<T> : IDomainService<T> where T:BaseEntity, new()
    {
        private IRepository<T> repository; 

        public BaseDomainService(IRepository<T> repository)
        {
            this.repository = repository;
        }

        public virtual IQueryable<T> GetAll()
        {
            return repository.GetAll();
        }

        public virtual async Task<IQueryable<T>> GetAllAsync()
        {
            return repository.GetAll();
        }

        public virtual IQueryable<T> GetAll(string name)
        {
            return repository.GetAll().Where(x => x.Name == name);
        }

        public virtual async Task<T> Get(long id)
        {
           return await GetAll().SingleOrDefaultAsync(x => x.Id == id);
        }

        public virtual Task<DataResult> Update(T entity, bool deferred = false)
        {
            return repository.Update(entity, deferred);
        }

        public virtual Task<DataResult> Create(T entity)
        {
            return repository.Create(entity);
        }

        public virtual Task<DataResult> Delete(long id)
        {
            return repository.Delete(id);
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await GetAll().AnyAsync(x=>x.Id == id);
        }

        public bool Exists(long id)
        {
            return GetAll().Any(e => e.Id == id);
        }

        public DbEntityEntry<T> Entry(T entity)
        {
            return repository.Entry(entity);
        }

       public async Task<DataResult> SaveChangesAsync()
        {
            var result = await repository.SaveChangesAsync();

            return DataResult.Ok(result);
        }
    }
}
