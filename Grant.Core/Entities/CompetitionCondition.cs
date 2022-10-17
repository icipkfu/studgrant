namespace Grant.Core.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Base;

    public class CompetitionCondition : BaseEntity
    {
        public string Description { get; set; }

        public ICollection<DataFile> Attachments { get; set; }
    }
}