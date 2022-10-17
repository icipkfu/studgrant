using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Entities
{
    using Base;

    public class UniversityCurator : BaseEntity
    {
        public University University { get; set; }
        public long UniversityId { get; set; }
        public Student Student { get; set; }
        public long StudentId { get; set; }
    }

}
