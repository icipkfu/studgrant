using System.ComponentModel.DataAnnotations;

namespace Grant.Core.Enum
{
    public enum AchievementCriterion
    {

        [Display(Name = "Получение студентом награды (приза)")]
        Award = 0,

        [Display(Name = "Проведение публичной культурно-творческой деятельности")]
        EventMaster = 1,

        [Display(Name = "Проведение спортивных мероприятий")]
        SportEvent = 2,

        [Display(Name = "Выполнение нормативов и требований ГТО")]
        Gto = 3,
        
        [Display(Name = "Получение студентом патента на изобретение")]
        Patent = 4,

        [Display(Name = "Получение студентом гранта на выполнение научно-исследовательской работы")]
        GrantResearchWork = 5,
    
        [Display(Name = "Наличие у студента публикации в научном издании в течение года")]
        ScientificPublication = 6,

        [Display(Name = "Наличие у студента публикации в научном издании в течение года во всероссийских изданиях (ВАК, РИНЦ)")]
        ScientificPublicationRu = 7,      

        [Display(Name = "Наличие у студента публикации в научном издании в течение года в международных изданиях(Scopus, Web of Science, ERIH PLUS")]
        ScientificPublicationWorld = 8

    }
}
