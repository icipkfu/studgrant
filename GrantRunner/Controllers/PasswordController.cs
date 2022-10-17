namespace Grant.WebApi.Controllers
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Grant.Utils.Extensions;
    using Microsoft.AspNet.Identity;
    using Services.DomainService;

    [RoutePrefix("api/psw/{studentId}")]
    public class PasswordController : BaseController
    {
        private readonly IStudentService _studentDomain;

        public PasswordController()
        {
            _studentDomain = this.Container.Get<IStudentService>();
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> ChangePassword(long studentId, string oldPassword, string newPassword, string confirmNewPassword)
        {
            if (oldPassword.IsEmpty())
            {
                return this.Ok();
            }

            if (!Equals(newPassword, confirmNewPassword))
            {
                return this.Ok();
            }

            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var student = await _studentDomain.GetAll()
                .FirstOrDefaultAsync(x => x.UserIdentityId == userId);

            if (student == null)
            {
                return this.BadRequest();
            }

            var usermanager = Startup.UserManagerFactory();

            await usermanager.ChangePasswordAsync(userId, oldPassword, newPassword);

            return this.Ok();
        }
    }
}