using System.Web.Http.Description;
using Grant.Core;

namespace Grant.WebApi.Controllers
{

    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Grant.Core.Entities;
    using Grant.Services.DomainService;
    using Grant.Utils.Extensions;

   [Authorize]
    public class GrantStudentController : BaseController
    {
        private IGrantStudentService _service;

        private IGrantStudentService service
        {
            get { return (_service ?? (_service = Container.Get<IGrantStudentService>())); }
        }

        private readonly IStudentService _studentService;

        public GrantStudentController()
        {
            _studentService = this.Container.GetInstance<IStudentService>();
        }

        [HttpGet]
        [Route("api/grantstudent")]
        public async Task<IQueryable<GrantStudent>> GetGrantStudents()
        {
            return await service.GetAllAsync();
        }

        [Route("api/grantstudent/{id}/{univerId}/{social}/{active}")]
        [ResponseType(typeof(GrantStudent))]
        public async Task<DataResult> GetGrantStudents(long id, long univerId, bool social, bool active)
        {
            GrantStudentFilter filter = new GrantStudentFilter { social = social, active = active };


            var quota = await service.GetGrantStudents(id, univerId, filter);

            if (quota == null)
            {
                return DataResult.Failure("Нет данных");
            }

            return quota;
        }

        [Route("api/grantstudent/{id}/{univerId}")]
        [ResponseType(typeof(GrantStudent))]
        [HttpPut]
        public async Task<DataResult> GetGrantStudentsFiletr(long id, long univerId, [FromBody] GrantStudentFilter filter)
        {
            var quota = await service.GetGrantStudents(id, univerId, filter);

            if (quota == null)
            {
                return DataResult.Failure("Нет данных");
            }

            return quota;
        }

        [Route("api/grantstudent/getunivers/{id}")]
        [ResponseType(typeof(University))]
        public async Task<IHttpActionResult> GetGrantUnivers(long id, [FromUri]bool additional = false)
        {
            var quota = await service.GetGrantUnivers(id, additional);

            if (quota == null)
            {
                return this.NotFound();
            }

            return this.Ok(quota);
        }

        [HttpPut]
        [Route("api/grantstudent/additional")]
        [ResponseType(typeof(GrantStudent))]
        public async Task<DataResult> GetAdditionalStudents([FromBody] AdditionalWinnerFilter filter)
        {
            var quota = await service.GetAdditionalStudents(filter);

            if (quota == null)
            {
                return DataResult.Failure("Нет данных");
            }

            return quota;
        }

        [HttpGet]
        [Route("api/grantstudent/winner/{id}/{studentId}")]
        public async Task<IHttpActionResult> ChooseWinner(long id, long studentId)
        {
            var result = await service.ChooseWinner(id, studentId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/grantstudent/cancelwinner/{id}/{studentId}")]
        public async Task<IHttpActionResult> CancelWinner(long id, long studentId)
        {
            var result = await service.CancelWinner(id, studentId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/grantstudent/additionalwinner/{id}/{studentId}")]
        public async Task<IHttpActionResult> ChooseAdditionalWinner(long id, long studentId)
        {
            var result = await service.ChooseAdditionalWinner(id, studentId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("api/grantstudent/canceladditionalwinner/{id}/{studentId}")]
        public async Task<IHttpActionResult> CancelAdditionalWinner(long id, long studentId)
        {
            var result = await service.CancelAdditionalWinner(id, studentId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/grantstudent/declinegrant/{id}")]
        public async Task<IHttpActionResult> DeclineGrant(long id)
        {
            var result = await service.DeclineGrant(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/grantstudent/agreegrant/{id}/{code}")]
        public async Task<IHttpActionResult> AcceptGrant(long id, string code)
        {
            var result = await service.AcceptGrant(id, code == "a", code=="h");

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/grantstudent/isparticipant/{id}")]
        public async Task<bool> IsParticipant(long id)
        {
            var result = await service.IsParticipant(id);

            return result;
        }

        [HttpGet]
        [Route("api/grantstudent/getstudentsreport/{grantid}/{univerId}")]
        public async Task<IHttpActionResult> GetStudentsReport(long grantid, long univerId, [FromUri]bool additional = false)
        {
            var result = await service.GetStudentsReport(grantid, univerId, additional);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("api/grantstudent/getalladditionalstudentsreport/{grantid}")]
        public async Task<IHttpActionResult> GetAllAdditionalReport(long grantid, [FromUri]  bool? onlyNewWinners = null)
        {
            var result = await service.GetAllAdditionalReport(grantid, onlyNewWinners);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpGet]
        [Route("api/grantstudent/getusergrants/{id}")]
        public async Task<DataResult> GetUserGrants(long id)
        {
            return await service.GetUserGrants(id);
        }
       

        [HttpGet]
        [Route("api/grantstudent/getwinnersreport/{grantid}")]
        public async Task<IHttpActionResult> GetWinnersReport(long grantid, [FromUri]bool additional, [FromUri]  bool? onlyNewWinners = null)
        {
            var result = await service.GetFullGrantReport(grantid, onlyNewWinners, additional);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("api/grantstudent/getmainwinnersreport/{grantid}")]
        public async Task<IHttpActionResult> GetMainWinnersReport(long grantid, [FromUri]bool additional = false, [FromUri]bool? onlyNewWinners = null)
        {
            var result = await service.GetMainFullGrantReport(grantid, onlyNewWinners, additional);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [Route("api/grantstudent/getwinnersotherreport/{grantid}/{activ}")]
        public async Task<IHttpActionResult> GetWinnersReport(long grantid, long activ, [FromUri]bool? onlyNewWinners = null)
        {
            var result = await service.GetFullGrantOtherReport(grantid, onlyNewWinners, 0, activ);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }



        [HttpGet]
        [Route("api/grantstudent/getwinnerslist/{grantid}")]
        public async Task<IHttpActionResult> GetWinnersList(long grantid, [FromUri]bool? onlyNewWinners = null)
        {
            var result = await service.GetWinnersList(grantid, onlyNewWinners);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("api/grantstudent/getstudentsdbfreport/{grantid}/{univerId}")]
        public async Task<IHttpActionResult> GetStudentsDbfReport(long grantid, long univerId, [FromUri]bool additional = false, [FromUri] bool? onlyNewWinners = null )
        {
            var result = await service.GetStudentsDbfReport(grantid, onlyNewWinners, univerId, additional);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }


        [HttpGet]
        [Route("api/grantstudent/getstat/{grantid}")]
        public async Task<IHttpActionResult> GetStat(long grantid)
        {
            var result = await service.GetStat(grantid);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
