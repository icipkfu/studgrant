using System.IO;
using System.Threading.Tasks;
using Grant.Core.Reports;
using Stimulsoft.Report;

namespace Grant.Reports.Reports.Base
{
    public class BaseStimulReport : IReport
    {
        protected BaseStimulReport()
        {
            Report = new StiReport();
        }
        
        /// <summary> Формат экспорта </summary>
        public StiExportFormat ExportFormat { get; set; }

        /// <summary> Отчёт </summary>
        public StiReport Report { get; set; }

        public virtual Stream GetTemplate()
        {
            throw new System.NotImplementedException();
        }

        public virtual string GetFileName()
        {
            throw new System.NotImplementedException();
        }

        public virtual void PrepareReport()
        {
            throw new System.NotImplementedException();
        }

        public virtual Task PrepareReportAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
