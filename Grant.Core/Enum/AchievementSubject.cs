using System.ComponentModel.DataAnnotations;

namespace Grant.Core.Enum
{
    public enum AchievementSubject
    {
        /// <summary>
        /// Наука
        /// </summary>
        [Display(Name = "Научно-исследовательская деятельность")]
        Science = 0,

        /// <summary>
        /// Спорт
        /// </summary>
        [Display(Name = "Спортивная деятельность")]
        Sport = 1,

        /// <summary>
        /// Творчество
        /// </summary>
        [Display(Name = "Культурно-творческая")]
        Creation = 2,

        /// <summary>
        /// Общественная деятельность
        /// </summary>
        [Display(Name = "Общественная деятельность")]
        SocialActivity = 3,

        [Display(Name = "Государственные награды, знаки отличия и иные формы поощрения")]
        StateAwards = 4,

    }
}