using System;
using Grant.Core.Entities.Base;

namespace Grant.Core.Entities
{
    using System.ComponentModel.DataAnnotations;
    using AspNet.Identity.PostgreSQL;
    using Enum;

    /// <summary>
    /// Сущность "Событие"
    /// </summary>
    public class GrantEvent : BaseEntity
    {
        /// <summary>
        /// Идентификатор гранта
        /// </summary>
        public long GrantId { get; set; }

        /// <summary>
        /// Грант
        /// </summary>
        public Grant Grant { get; set; }
        
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public IdentityUser User { get; set; }

        /// <summary>
        /// Идентификатор студента
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// Студент
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Подзаголовок
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Изображение
        /// </summary>
        public string Image { get; set; } 

        /// <summary>
        /// Цвет события
        /// </summary>
        public string Palette { get; set; }

        /// <summary>
        /// Содержимое
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Условия
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// Вложения
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        public string DateChange { get; set; }

        /// <summary>
        /// Квота изменилась
        /// </summary>
        public string QuotaChanged { get; set; }

        /// <summary>
        /// Администратор изменился
        /// </summary>
        public string ChangeAdmin { get; set; }

        /// <summary>
        /// Тип события
        /// </summary>
        [Required]
        public EventType EventType { get; set; }
    }
}
