using System.IO;
using System.Threading.Tasks;

namespace Grant.Core.Reports
{
    /// <summary>
    /// Интерфейс отчета
    /// </summary>
    public interface IReport
    {
        /// <summary>
        /// Получить шаблон
        /// </summary>
        Stream GetTemplate();

        /// <summary> 
        /// Имя файла. Обязательно с расширением.
        /// </summary>
        string GetFileName();

        /// <summary>
        /// Выполнить сборку отчета
        /// </summary>
        Task PrepareReportAsync();

        /// <summary>
        /// Выполнить сборку отчета
        /// </summary>
        void PrepareReport();
    }
}