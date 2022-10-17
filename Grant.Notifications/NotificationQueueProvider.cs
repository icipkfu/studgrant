using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Grant.Core.Entities;
using Grant.Core.Notification;
using Grant.Core.UserIdentity;
using Grant.DataAccess;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Grant.Notifications
{
    public class NotificationQueueProvider : INotificationQueueProvider
    {
        private readonly INotificator _notificator;
        private readonly IRepository<NotificationQueue> _queueRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Core.Entities.Grant> _grantRepository;

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? (_userManager = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>());
            }
        }

        public NotificationQueueProvider(INotificator notificator, IRepository<NotificationQueue> queueRepository, IRepository<Student> studentRepository, 
            IRepository<Core.Entities.Grant> grantRepository)
        {
            _notificator = notificator;
            _queueRepository = queueRepository;
            _studentRepository = studentRepository;
            _grantRepository = grantRepository;
        }

        public async Task Enqueue(Student student, Core.Entities.Grant grant, NotificationType type, Dictionary<string, object> parameters)
        {
            var email = await UserManager.GetEmailAsync(student.UserIdentityId);

            await _queueRepository.Create(new NotificationQueue
            {
                StudentId = student.Id,
                GrantId = grant != null ? grant.Id : 21,
                NotificationType = type,
                Email = email,
                Parameters = JsonConvert.SerializeObject(parameters)
            });
        }

        public async Task<bool> CheckQueue()
        {
            return await _queueRepository.GetAll().AnyAsync(x => !x.Sent);
        }

        public async Task SentQueue()
        {
            var notifications = await _queueRepository.GetAll()
                .Where(x => !x.Sent)
                .OrderBy(x => x.CreateDate)
                .ToListAsync();

           foreach (var notification in notifications)
            {
                notification.Student = await _studentRepository.GetAll().SingleOrDefaultAsync(x => x.Id == notification.StudentId);
                notification.Grant = await _grantRepository.GetAll().SingleOrDefaultAsync(x => x.Id == notification.GrantId);
                var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(notification.Parameters);


                if (notification.NotificationType == NotificationType.InvalidData)
                {
                   var result =  await _notificator.NotifyUser(notification.Email, notification.NotificationType, parameters);

                    if (result.Success)
                    {
                        notification.Sent = true;
                    }
                    else
                    {
                        notification.SendError = result.Message;
                    }

                    notification.Student = null;
                    notification.Grant = null;

                    await _queueRepository.Update(notification);


                    var test = "";
                }
                
               
            }  
        }
    }
}
