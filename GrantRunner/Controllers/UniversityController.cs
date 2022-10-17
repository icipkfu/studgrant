using System.Collections.Generic;
using Grant.Core;
using Grant.Core.Enum;

namespace Grant.WebApi.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Grant.Core.Entities;
    using Grant.Services.DomainService;
    using Grant.Utils.Extensions;

     [Authorize]
    public class UniversityController : BaseController
    {
        private IUniversityService _service;

        private IBankFilialService _service1;


        public UniversityController()
        {
            _service1 = this.Container.GetInstance<IBankFilialService>();
        }

        private IUniversityService service
        {
            get { return (_service ?? (_service = Container.Get<IUniversityService>())); }
        }

//         GET api/Universities
         [HttpGet]
        [Route("api/University")]
        public async Task<IQueryable<University>> GetUniversities()
         {
             return await service.GetAllAsync();
         }

         [HttpGet]
         [Route("api/allUniversity")]
         public async Task<IQueryable<University>> GetAllUniversities()
         {
             return await service.GetAllUniversities(null);
         }


         [HttpPut]
         [Route("api/filteredUniversity")]
         public async Task<IEnumerable<University>> GeFilteredUniversities([FromBody] UniversityFilter filter)
         {
             var result = await service.GetAllUniversities(filter);

             return result;
         }

        [HttpGet]
        [Route("api/University/{name}")]
        [ResponseType(typeof(University))]
        public IQueryable<University> GetUniversities([FromUri]string name)
        {
            return service.GetAll().Where(x=> x.Name.ToUpper().Contains(name.ToUpper()));
        }

        [HttpGet]
        [Route("api/Universities/{name}")]
        [ResponseType(typeof(University))]
        public IQueryable<University> GetUniversity([FromUri]string name)
        {
            if (name != "null")
            {
                return service.GetAll().Where(x => x.Name.ToUpper().Contains(name.ToUpper())).OrderBy(x => x.Name);
            }

            return service.GetAll().OrderBy(x => x.Name);
        }

       

        [HttpGet]
        [Route("api/University/getbyid/{id}")]
        [ResponseType(typeof(University))]
        public async Task<IHttpActionResult> GetUniversity(long id)
        {
            var university = await service.Get(id); 

            if (university == null)
            {
                return this.NotFound();
            }

            return this.Ok(university);
        }


        [HttpPut]
        [Route("api/university/{id}")]
        public async Task<IHttpActionResult> PutUniversity(long id, University university)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != university.Id)
            {
                return this.BadRequest();
            }

            await service.Update(university);

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Universities
        [HttpPost]
        [ResponseType(typeof(University))]
        [Route("api/University")]
        public async Task<IHttpActionResult> PostUniversity(University university)
        {
            if (string.IsNullOrEmpty(university.Name) || !this.ModelState.IsValid )
            {
                return this.BadRequest(this.ModelState);
            }

            var result = await service.Create(university);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(university);
        }

        // DELETE api/Universities/5
        [HttpDelete]
        [Route("api/university/{id}")]
        [ResponseType(typeof(University))]
        public async Task<IHttpActionResult> DeleteUniversity(long id)
        {
            var result = await service.Delete(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }
    }
}