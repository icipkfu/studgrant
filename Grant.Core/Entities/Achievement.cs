namespace Grant.Core.Entities
{
    using Base;
    using Enum;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Достижение
    /// </summary>
    public class Achievement : BaseEntity
    {
        /// <summary>
        /// Студент
        /// </summary>
        [JsonIgnore]
        public Student Student { get; set; }

        public long StudentId { get; set; }

        public ValidationState ValidationState { get; set; }

        public string ValidationComment { get; set; }

        /// <summary>
        /// Направление (тематика)
        /// </summary>
        public AchievementSubject Subject { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public AchievementState? State { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public AchievementLevel? Level { get; set; }


        /// <summary>
        /// Статус
        /// </summary>
        public AchievementCriterion? Criterion { get; set; }

        /// <summary>
        /// Год
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Гуиды файлов строкой через разделитель '|'
        /// </summary>
        public string Files { get; set; }

        public string ProofFile { get; set; }

        public int Score { get; set; }

        [NotMapped]
        public List<Core.DataFileResult> FilesList { get; set; }

        [NotMapped]
        public List<Core.DataFileResult> ProofList { get; set; }

        [NotMapped]
        public string ImageLink { get; set; }

        [NotMapped]
        public bool IsNew { get; set; }

        
    }
}