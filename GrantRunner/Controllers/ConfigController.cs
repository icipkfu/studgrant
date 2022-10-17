using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Grant.Core;
using Grant.Core.Context;
using Grant.Core.Enum;
using Grant.Services.DomainService;
using Grant.Utils.Extensions;

namespace Grant.WebApi.Controllers
{
    [Authorize]
    public class ConfigController : BaseController
    {
        private IGrantEventService eventService;
        private IGrantService grantService;

        public ConfigController()
        {
            this.eventService = Container.Get<IGrantEventService>();
            grantService = Container.Get<IGrantService>();
        }

        [HttpGet]
        [Route("api/config/getbyname")]
        public async Task<DataResult> GetByName()
        {
            var filename = ApplicationContext.Current.MapPath("userscantedit");

            if (File.Exists(filename))
            {
                return DataResult.Ok(true);
            }
            else
            {
                return DataResult.Ok(false);
            }
        }


        [HttpGet]
        [Route("api/config/setbyname/{value}")]
        public async Task<DataResult> SetByName(string value)
        {

            var val = value.ToBool();

            var filename = ApplicationContext.Current.MapPath("userscantedit");

            if (val)
            {
                if (!File.Exists(filename))
                {
                    var grant = await grantService.Get(21);
                    await eventService.CreateEvent(grant, "Приостановлено изменение данных",
                        "Приостановлено изменение данных пользователей", EventType.UserDataReadonly);

                    using (FileStream fs = File.Create(filename))
                    {
                        
                    }
                }
            } 
            else
            {
                if (File.Exists(filename))
                {
                    try
                    {
                        var grant = await grantService.Get(21);
                        await eventService.CreateEvent(grant, "Возобновлено изменение данных",
                            "Возобновлено изменение данных пользователей", EventType.UserDataEditable);

                        File.Delete(filename);
                    }
                    catch (Exception ex)
                    {
                        return DataResult.Failure(ex.Message);
                    }
                }
            }

            return DataResult.Ok();
        }
	}
}