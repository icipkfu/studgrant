namespace Grant.WebApi.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Entities;
    using Services.DomainService;

    [RoutePrefix("api/personalInfoData/{fileHash}")]

    public class PersonalInfoDataController : BaseController
    {
        private readonly IStudentService _studentDomain;
        private readonly IDomainService<PersonalInfo> _domain;
        private readonly IPersonalInfoService _personalInfoService;

        public PersonalInfoDataController()
        {
            this._domain = this.Container.GetInstance<IDomainService<PersonalInfo>>();
            this._studentDomain = this.Container.GetInstance<IStudentService>();
            this._personalInfoService = this.Container.GetInstance<IPersonalInfoService>();
        }
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> DeleteFile(string fileHash)
        {
            var info = await this.GetCurrentInfo();
            var success = await this._personalInfoService.DeleteFile(info, fileHash);

            info.PassportScanLinks = Utils.GetFilesListFromRow(info.PassportScan, '|');
                        //this._personalInfoService.GetPassportScanLinks(info.PassportScan);

            if (!success)
            {
                return this.BadRequest("Во время удаления произошли ошибки.");
            }

            return this.Ok(info);
        }

        private async Task<PersonalInfo> GetCurrentInfo()
        {
            var studentId = (await this._studentDomain.GetCurrent()).Id;

            return
                await
                    this._studentDomain.GetAll()
                        .Where(x => x.Id == studentId)
                        .Select(x => x.PersonalInfo)
                        .FirstOrDefaultAsync();
        }
    }
}
