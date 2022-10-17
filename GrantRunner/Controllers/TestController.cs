using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Grant.Core.Config;
using Grant.Core.Entities;
using Grant.Core.Enum;
using Grant.Reports.Reports;
using Grant.Services;
using Grant.Services.DomainService;

namespace Grant.WebApi.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core.Context;
    using Core.Notification;
    using Grant.Utils.Extensions;
    using Grant.Core;
    using Grant.Core.Smtp;
    using System.Net.Mail;

    public class TestController : BaseController
    {
        private IAchievementService achievementService;
        private IStudentService studentService;
        private IAchievementScore achService;
        private IDomainService<PersonalInfoHistory> histDomain;
        private IFileManager fileManager;
            
        TestController()
        {
            this.achService = Container.Get<IAchievementScore>();
            studentService = Container.Get<IStudentService>();
            achievementService = Container.Get<IAchievementService>();
            histDomain = Container.Get<IDomainService<PersonalInfoHistory>>();
            fileManager = Container.Get<IFileManager>();
        }


        [HttpGet]
        [Route("test/index")]
        public async Task<IHttpActionResult> Index()
        {
            var students =
                await
                    studentService.GetAll()
                       // .Include(x => x.PersonalInfo)
                       // .Where(x => x.PersonalInfo.PassportSeries != null && x.PersonalInfo.PassportSeries.Length == 4)
                        .ToListAsync();



            /*achievementService.GetAll().Where(x=>x.Year >= 2014)
                .Include(x => x.Student)
                //.Where(x=>x.Student.Id == 34)
                .GroupBy(x => x.Student)
                .ToDictionaryAsync(x => x.Key, y => y.ToList()); */
            //studentService.GetAll().Include(x=>x.Achievements)
            //    .Where(
            //        x => x.Id == 34)
            //    .ToListAsync();


            var i = 0;

            var j = 1000;

            var emailSender = Container.Get<IMailSender>();

            foreach (var obj in students)
            {

                await achievementService.UpdateScore(obj.Id);                

                if (i == j)
                {
                    Attachment[] arr2 = new List<Attachment>().ToArray();                    
                    j += 2000;
                }

                i++;
            }

           
            Attachment[] arr = new List<Attachment>().ToArray();

            return this.Ok();
        }
    }
}