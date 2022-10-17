namespace Grant.Core.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Base;
    using Enum;

    /// <summary>
    /// Событие
    /// </summary>
    public class Event : BaseEntity
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        [Required]
        public string Title { get; set; }
        
        /// <summary>
        /// Подзаголовок
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Содержимое
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// Изображение
        /// </summary>
        public string Image { get; set; } 
        
        /// <summary>
        /// Идентификатор студента
        /// </summary>
        public long StudentId { get; set; } 

        /// <summary>
        /// Студент
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Тип события
        /// </summary>
        public EventType EventType { get; set; }
        
    }
}