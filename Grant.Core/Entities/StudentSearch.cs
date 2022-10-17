using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grant.Core.Enum;

namespace Grant.Core.Entities
{
    [NotMapped]
    public class StudentFilter
    {
        public string Name { get; set; }

        public long? GrantId { get; set; }

        public long? UniversityId { get; set; }

        public Citizenship? Citizenship { get; set; }

        public ValidationState? PersonalData { get; set; }

        public ValidationState? RecordBook { get; set; }

        public ValidationState? Income { get; set; }

        public long lastId { get; set; }

        public int sortBy { get; set; }

        public int skip { get; set; }

        public bool Asc { get; set; }

        public bool? IsWinner { get; set; }

        public bool? IsPassportOutDate { get; set; }
    }

    [NotMapped]
    public class EventFilter
    {
        public int? Type { get; set; }

        public long UserId { get; set; }

        public DateTime? After { get; set; }

        public DateTime? Before { get; set; }

        public long LastId { get; set; }

        public string Search { get; set; }
    }

    [NotMapped]
    public class UniversityFilter
    {
        public int? Type { get; set; }

        public string Search { get; set; }
    }

    [NotMapped]
    public class AdditionalWinnerFilter
    {
        public string Name { get; set; }

        public long? GrantId { get; set; }

        public long? UniversityId { get; set; }

        public long lastId { get; set; }

        public int sortBy { get; set; }

        public int skip { get; set; }

        public bool Asc { get; set; }
    }

    [NotMapped]
    public class GrantStudentFilter
    {
        public ValidationState? selectedRecordbook { get; set; }

        public ValidationState? selectedAchievements { get; set; }

        public ValidationState? selectedScore { get; set; }

        public ValidationState? selectedIncome { get; set; }

        public bool social { get; set; }
        public bool active { get; set; }
    }
}
