using System.IO;
using System.Threading.Tasks;

namespace Grant.Reports.Reports
{
    public interface IDbfReportGenerator
    {
        MemoryStream GetDbfReport(long grantId, string town, long universityId, bool additional, bool byUniversity, bool? onlyNewWinners);

        Task<MemoryStream> GetDbfReportAsync(long grantId, string town, long universityId, bool additional, bool byUniversity, bool? onlyNewWinners);
    }
}