using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grant.Core.Entities;

namespace Grant.Services.DomainService
{
    public interface IAchievementScore
    {
        int GetScore(ICollection<Achievement> list);
    }
}
