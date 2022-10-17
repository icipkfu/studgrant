namespace Grant.WebApi.Controllers
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using AspNet.Identity.PostgreSQL;
    using Core.DbContext;
    using Grant.Utils.Extensions;

     [Authorize]
    public class UserController : ApiController
    {
        private readonly GrantDbContext _db = new GrantDbContext();

        // GET api/User
        public IQueryable<IdentityUser> GetUsers()
        {
            return this._db.Users;
        }

        // GET api/User/5
        [ResponseType(typeof(IdentityUser))]
        public async Task<IHttpActionResult> GetUser(long id)
        {
            var user = await this._db.Users.FindAsync(id);
            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user);
        }

        // PUT api/User/5
        public async Task<IHttpActionResult> PutUser(string id, IdentityUser user)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != user.Id)
            {
                return this.BadRequest();
            }

            this._db.Entry(user).State = EntityState.Modified;

            try
            {
                await this._db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.UserExists(id))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/User
        [ResponseType(typeof(IdentityUser))]
        public async Task<IHttpActionResult> PostUser(IdentityUser user)
        {
            //user.Login = "fake";
            user.Email = "fake@fake.fake";
            user.PasswordHash = "fAkE";
            //if (!this.ModelState.IsValid)
            //{
            //    return this.BadRequest(this.ModelState);
            //}

            this._db.Users.Add(user);
            await this._db.SaveChangesAsync();

            return this.CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE api/User/5
        [ResponseType(typeof(IdentityUser))]
        public async Task<IHttpActionResult> DeleteUser(long id)
        {
            var user = await this._db.Users.FindAsync(id);
            if (user == null)
            {
                return this.NotFound();
            }

            this._db.Users.Remove(user);
            await this._db.SaveChangesAsync();

            return this.Ok(user);
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<IHttpActionResult> ChangePassword(string id, string oldPassword, string newPassword, string confirmNewPassword)
        {
            if (id.IsEmpty() || oldPassword.IsEmpty()
                || newPassword.IsEmpty() || confirmNewPassword.IsEmpty())
            {
                return this.BadRequest();
            }

            if (newPassword != confirmNewPassword)
            {
                return this.Json(new {success = false, message = "Invalid password"});
            }

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                this.Json(new {success = false, message = "User not found"});
            }

            return this.Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(string id)
        {
            return this._db.Users.Count(e => e.Id == id) > 0;
        }
    }
}