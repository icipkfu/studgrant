using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grant.Core.Enum;
using Grant.Core.Entities.Base;

namespace Grant.Core.Entities
{
    public class ValidationHistory : BaseEntity
    {
        public Student Moderator { get; set; }

        public long ModeratorId { get; set; }

        public Student ValidationUser { get; set; }

        public long ValidationUserId { get; set; }

        public ValidationState State { get; set; }

        public ValidationTarget Target {get; set;}

        public string ValidationMessage {get; set; }
    }
}
