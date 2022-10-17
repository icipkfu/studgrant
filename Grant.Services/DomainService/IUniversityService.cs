namespace Grant.Services.DomainService
{
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities;

    public interface IUniversityService : IDomainService<University>
    {
        Task<IQueryable<University>> GetAllUniversities(UniversityFilter filter);
    }
}
