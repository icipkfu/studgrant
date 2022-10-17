using System.IO;
using System.Threading.Tasks;

namespace Grant.Core.Reports
{
    /// <summary>
    /// Интерфейс генератора отчетов
    /// </summary>
    public interface IReportGenerator
    {
        /// <summary>
        /// Метод генерации отчета по заданным параметрам
        /// </summary>
        /// <param name="result">
        /// Поток результата 
        /// </param>
        /// <param name="report">
        /// Генерируемый отчет
        /// </param>
        Task GenerateAsync(Stream result, IReport report);

        Task GenerateDocAsync(Stream result, IReport report);


        void Generate(Stream result, IReport report);

        void GenerateDoc(Stream result, IReport report);
    }
}