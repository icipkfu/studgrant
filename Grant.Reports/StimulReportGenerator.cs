using System;
using System.IO;
using System.Threading.Tasks;
using Grant.Core.Reports;
using Grant.Reports.Reports.Base;
using Stimulsoft.Report;

namespace Grant.Reports
{
    public class StimulReportGenerator : IReportGenerator
    {
        public async Task GenerateAsync(Stream result, IReport report)
        {
            var stimulReport = report as BaseStimulReport;
            if (stimulReport == null)
            {
                throw new ArgumentException("report is not convertible to BaseStimulReport", "report");
            }

            stimulReport.ExportFormat = StiExportFormat.Pdf;
            stimulReport.Report.Load(stimulReport.GetTemplate());
            stimulReport.Report.Compile();
            await stimulReport.PrepareReportAsync();

            if (!stimulReport.Report.IsRendered)
            {
                stimulReport.Report.Render();
            }

            var ms = new MemoryStream();
            result.Seek(0, SeekOrigin.Begin);
            stimulReport.Report.ExportDocument(stimulReport.ExportFormat, result);
            ms.Seek(0, SeekOrigin.Begin);
        }


        public async Task GenerateDocAsync(Stream result, IReport report)
        {
            var stimulReport = report as BaseStimulReport;
            if (stimulReport == null)
            {
                throw new ArgumentException("report is not convertible to BaseStimulReport", "report");
            }

            stimulReport.ExportFormat = StiExportFormat.Word2007;
            stimulReport.Report.Load(stimulReport.GetTemplate());
            stimulReport.Report.Compile();
            await stimulReport.PrepareReportAsync();

            if (!stimulReport.Report.IsRendered)
            {
                stimulReport.Report.Render();
            }

            var ms = new MemoryStream();
            result.Seek(0, SeekOrigin.Begin);
            stimulReport.Report.ExportDocument(stimulReport.ExportFormat, result);
            ms.Seek(0, SeekOrigin.Begin);
        }

        public void GenerateDoc(Stream result, IReport report)
        {
            var stimulReport = report as BaseStimulReport;
            if (stimulReport == null)
            {
                throw new ArgumentException("report is not convertible to BaseStimulReport", "report");
            }

            stimulReport.ExportFormat = StiExportFormat.Word2007;
            stimulReport.Report.Load(stimulReport.GetTemplate());
            stimulReport.Report.Compile();
            stimulReport.PrepareReport();

            if (!stimulReport.Report.IsRendered)
            {
                stimulReport.Report.Render();
            }

            var ms = new MemoryStream();
            result.Seek(0, SeekOrigin.Begin);
            stimulReport.Report.ExportDocument(stimulReport.ExportFormat, result);
            ms.Seek(0, SeekOrigin.Begin);
        }


        public void Generate(Stream result, IReport report)
        {
            var stimulReport = report as BaseStimulReport;
            if (stimulReport == null)
            {
                throw new ArgumentException("report is not convertible to BaseStimulReport", "report");
            }

            stimulReport.ExportFormat = StiExportFormat.Pdf;
            stimulReport.Report.Load(stimulReport.GetTemplate());
            stimulReport.Report.Compile();
            stimulReport.PrepareReport();

            if (!stimulReport.Report.IsRendered)
            {
                stimulReport.Report.Render();
            }

            var ms = new MemoryStream();
            result.Seek(0, SeekOrigin.Begin);
            stimulReport.Report.ExportDocument(stimulReport.ExportFormat, result);
            ms.Seek(0, SeekOrigin.Begin);
        }

    }
}
