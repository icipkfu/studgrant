using System.ComponentModel.DataAnnotations.Schema;
using Grant.Core.Entities.Base;

namespace Grant.Core.Entities
{
    public class GrantQuota : BaseEntity
    {
        public long GrantId { get; set; }

        public Grant Grant { get; set; }

        public long UniversityId { get; set; }

        public University University { get; set; }

        public int Quota { get; set; }

        public string WinnerReport { get; set; }

        public string AdditionalWinnerReport { get; set; }

        [NotMapped]
        public DataFileResult WinnerReportFile { get; set; }


        [NotMapped]
        public DataFileResult AdditionalWinnerReportFile { get; set; }

         [NotMapped]
        public int StudentCount { get; set; }

         [NotMapped]
         public int WinnerCount { get; set; }

         [NotMapped]
         public string Link { get; set; }
    }
}
