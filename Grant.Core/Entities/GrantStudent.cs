using System.ComponentModel.DataAnnotations.Schema;
using Grant.Core.Entities.Base;

namespace Grant.Core.Entities
{
    public class GrantStudent : BaseEntity
    {
        public long GrantId { get; set; }

        public Grant Grant { get; set; }

        public long UniversityId { get; set; }

        public University University { get; set; }

        public long StudentId { get; set; }

        public Student Student { get; set; }

        public bool IsWinner { get; set; }

        public bool IsAdditionalWinner { get; set; }

        public bool IsSocialActive { get; set; }

        public bool IsSocialHelp { get; set; }

        [NotMapped]
        public string thumb { get; set; }

        [NotMapped]
        public string Fio { get; set; }

        [NotMapped]
        public int Score { get; set; }

        [NotMapped]
        public bool IsPassportValid { get; set; }

        [NotMapped]
        public bool IsStudentBookValid { get; set; }

        [NotMapped]
        public string UniversityName { get; set; }

    }
}
