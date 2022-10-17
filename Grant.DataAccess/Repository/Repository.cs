using Grant.DataAccess;

namespace Grant.DataAccess
{
    using System.Linq;
    using System.Threading.Tasks;
    using Grant.Core;
    using Grant.Core.Entities.Base;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using Grant.Core.DbContext;

    public class Repository<T> : IRepository<T> where T:BaseEntity, new()
    {
        private IDbSet<T> entities;

        private ISession db;

        public Repository(ISession session)
        {
            this.db = session;
        }

        private IDbSet<T> Entities
        {
            get { return entities ?? (entities = db.CurrentContext().Set<T>()); }
        }

        public IQueryable<T> GetAll()
        {
            return Entities;
        }

        public async Task<DataResult> Update(T entity, bool deferred = false)
        {
            db.CurrentContext().Entry(entity).State = EntityState.Modified;

            if (!deferred)
            {
                try
                {
                    var result = await db.CurrentContext().SaveChangesAsync();
                    return DataResult.Ok(result);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (Exists(entity.Id))
                    {
                        throw;
                    }
                }  
            }

            return DataResult.Ok();
        }

        public async Task<DataResult> Create(T entity, bool deferred = false)
        {
            Entities.Add(entity);

            if (!deferred)
            {
               var result =  await db.CurrentContext().SaveChangesAsync();
               return DataResult.Ok(result);
            }
            return DataResult.Ok(entity);
        }

        public async Task<DataResult> Delete(long id, bool deferred = false)
        {
           var entity = await GetAll().Where(x => x.Id == id).SingleOrDefaultAsync();

            if (entity == null)
            {
                return DataResult.Failure("Отсутствует объект с данным Id");
            }

           Entities.Remove(entity);

           if (!deferred)
           {
               var result = await this.db.CurrentContext().SaveChangesAsync();
               return DataResult.Ok(result);
           }

           return DataResult.Ok();
        }

        public async Task<DataResult> SaveChangesAsync()
        {
            var result = await db.CurrentContext().SaveChangesAsync();

            return DataResult.Ok(result);
        }

        public bool Exists(long id)
        {
            return Entities.Any(e => e.Id == id);
        }

        public DbEntityEntry<T> Entry(T entity)
        {
            return db.CurrentContext().Entry(entity);
        }
    }
}
