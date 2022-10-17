using Grant.Core.Enum;

namespace Grant.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using Base;
    using AspNet.Identity.PostgreSQL;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Grant : BaseEntity
    {
        public string Description { get; set; }

        public DateTime ExpiresDate { get; set; }

        public int? FullQuota { get; set; }

        public CompetitionCondition CompetitionCondition { get; set; }

        public GrantStatus Status { get; set; }

        public ICollection<IdentityUser> MajorWinners { get; set; }

        public ICollection<IdentityUser> AdditionalWinners { get; set; }

        public string ImageFile { get; set; }

        public string AttachmentFiles { get; set; }

        public string Conditions { get; set; }

        public bool CanAddReport { get; set; }

        public string Administrators { get; set; }

        [NotMapped]
        public ICollection<Core.DataFileResult> AttachmentsLinks { get; set; }


        [NotMapped]
        public string ImageLink { get; set; }
    }
}