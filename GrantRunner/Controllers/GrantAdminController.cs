using System.Linq;

namespace Grant.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Core.Entities;
    using Grant.Utils.Extensions;
    using Services.DomainService;

        [Authorize]
    public class GrantAdminController : BaseController
    {
        private readonly IGrantAdminService _grantAdminService;

        private readonly IGrantService _grantService;
        

        public GrantAdminController()
        {
           _grantAdminService =  Container.GetInstance<IGrantAdminService>();
           _grantService = Container.GetInstance<IGrantService>();
        }

        //[Route("api/grant/admin/{id}")]
        [ResponseType(typeof (Student))]
        public async Task<IHttpActionResult> GetGrantAdmins(long id)
        {
            var admins = await _grantAdminService.GetGrantAdmins(id);

            if (admins == null)
            {
                return this.NotFound();
            }

            return this.Ok(admins);
        }

        //[HttpGet]
        ////[Route("api/grant/setadmin/{id}")]
        //public async Task<IHttpActionResult> SetGrantAdmins(string id, IEnumerable<long> students)
        //{
        //    long Id = id.ToLong();

        //    if (Id == 0)
        //    {
        //        return BadRequest("Нe задан ID");
        //    }

        //    var result = await _grantAdminService.SetGrantAdmins(Id, students.ToArray());

        //    if (!result.Success)
        //    {
        //        return BadRequest(result.Message);
        //    }

        //    return this.Ok();
        //}
    }
}