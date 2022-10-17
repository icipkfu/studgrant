using Grant.Core.Entities.Base;

namespace Grant.Core.Entities
{
    public class PersonalInfoFile : BaseEntity
    {
        public PersonalInfo PersonalInfo { get; set; }

        public string FileHash { get; set; }
    }
}
