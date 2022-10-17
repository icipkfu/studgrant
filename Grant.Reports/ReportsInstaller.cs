using Grant.Core.Reports;
using Grant.Install;
using Grant.Reports.Reports;
using Grant.Utils.Extensions;

namespace Grant.Reports
{
    public class ReportsInstaller : BaseInstaller
    {
        public override void Install()
        {
            this.Container.RegisterTransient<IReportGenerator, StimulReportGenerator>();
            this.Container.RegisterTransient<IReport, GrantStudentsReport>("GrantStudentsReport");
            this.Container.RegisterTransient<IReport, GrantWinnerReport>("GrantWinnerReport");
            this.Container.RegisterTransient<IDbfReportGenerator, DbfReportGenerator>();
        }
    }
}
