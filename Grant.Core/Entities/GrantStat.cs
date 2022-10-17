using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Entities
{
    [NotMapped]
    public class GrantStat
    {
        public int Quota { get; set; }

        public int UniversutyQuota { get; set; }

        public int StudentCount { get; set; }

        public int WinnersCount { get; set; }

        public int AdditionalWinnersCount { get; set; }

        public int NotRfStudentCount { get; set; }

        public int NotRfWinnerCount { get; set; }

        public int NotRfAddWinnerCount { get; set; }
    }

    [NotMapped]
    public class ContactInfo
    {
        public string UniversityName { get; set; }

        public string CuratorName { get; set; }

        public string CuratorPhone { get; set; }

        public string ZoneName { get; set; }

        public string ZoneManName { get; set; }

        public string ZoneManPhone { get; set; }

    }
}
