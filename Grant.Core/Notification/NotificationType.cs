using System.ComponentModel.DataAnnotations;

namespace Grant.Core.Notification
{
    public enum NotificationType
    {
        None = 0,
        Notification1 = 1,
        Notification2 = 2,
        Notification3 = 3,
        Notification4 = 4,
        Notification5 = 5,
        /// <summary>
        /// уведомление о регистрации
        /// </summary>
        [Display(Name = "Регистрация в системе")]
        Notification6 = 6,
        /// <summary>
        /// уведомление о создании гранта
        /// </summary>
        [Display(Name="Новый грант в системе")]
        GrantCreated = 7,
        [Display(Name = "Восстановление пароля")]
        ResetPasswordLink = 8,
        [Display(Name = "Новый пароль")]
        NewPassword = 9,

        [Display(Name = "Проголосуй")]
        SomeText = 11,

        [Display(Name = "Замечание к данным")]
        InvalidData = 10
    }
}