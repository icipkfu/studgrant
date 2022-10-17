namespace Grant.Services
{
    using Core.Entities.Base;
    using DomainService;

    public static class DomainServiceExtensions
    {
        public static void SaveOrUpdate<T>(this IDomainService<T> domain, T entity) where T: BaseEntity
        {
            if (entity.Id > 0)
            {
                domain.Update(entity);
            }
            else
            {
                domain.Create(entity);
            }
        }
    }
}