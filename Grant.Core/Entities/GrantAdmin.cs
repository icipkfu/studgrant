namespace Grant.Core.Entities
{
    using Base;

    /// <summary>
    /// Администратор гранта
    /// </summary>
    public class GrantAdmin : BaseEntity
    {
        /// <summary>
        /// Идентификатор студента
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// Студент
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Идентификатор гранта
        /// </summary>
        public long GrantId { get; set; }

        /// <summary>
        /// Грант
        /// </summary>
        public Grant Grant { get; set; }

    }
}
