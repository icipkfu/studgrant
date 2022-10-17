namespace Grant.Core.DbContext
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Grant.Core.Interfaces;

    public static class DbContextExtensions
    {
        public static Task<int> SoftDelete(this GrantDbContext context, IBaseEntity entity)
        {
            if (!entity.DeletedMark)
            {
                entity.DeletedMark = true;
                context.Entry(entity).State = EntityState.Modified;
                return context.SaveChangesAsync();
            }
            return new Task<int>(() => 0);
        }

        public static Task<int> SoftDelete(this GrantDbContext context, ICollection<IBaseEntity> entityCollection)
        {
            foreach (var entity in entityCollection.Where(entity => !entity.DeletedMark))
            {
                entity.DeletedMark = true;
                context.Entry(entity).State = EntityState.Modified;
            }
            return context.SaveChangesAsync();
        }
    }
}