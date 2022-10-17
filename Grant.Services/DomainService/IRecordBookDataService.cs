using Grant.Core;
using Grant.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Services.DomainService
{
    public interface IIncomeDataService
    {
        Task<DataResult> Delete(Student student, string hash);
    }
}
