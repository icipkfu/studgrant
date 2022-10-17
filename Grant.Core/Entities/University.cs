using Grant.Core.Enum;

namespace Grant.Core.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Base;

    public class University : BaseEntity
    {
        public string Address { get; set; }

        public string ImageFile { get; set; }

        public long CuratorId { get; set; }

        [NotMapped]
        public Student Curator { get; set; }

        [NotMapped]
        public string ImageLink { get; set; }

        [NotMapped]
        public string thumb { get; set; }

        [NotMapped]
        public string CuratorFio { get; set; }

         [NotMapped]
        public int RegisteredCount { get; set; }

        public string City { get; set; }

        public string Town { get; set; }

        public UniverType UniverType { get; set; }

        public BankFilial BankFilial { get; set; }

        public long? BankFilialId { get; set; }



    }
}