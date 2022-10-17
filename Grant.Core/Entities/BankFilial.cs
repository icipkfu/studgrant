using Grant.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Entities
{
    public class BankFilial: BaseEntity
    {
        public string Code { get; set; }

        public string FilialName { get; set; }

        public string Address { get; set; }
    }
}
