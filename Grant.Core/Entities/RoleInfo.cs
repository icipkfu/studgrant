using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grant.Core.Entities
{
    [NotMapped]
    public class RoleInfo
    {
        public long Id { get; set; }
        public Role? Role { get; set; }

        public List<long> GrantsAdmin { get; set; }

        public List<long> UniversCurator { get; set; }
    }
}
