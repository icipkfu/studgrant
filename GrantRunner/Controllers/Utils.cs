namespace Grant.WebApi.Controllers
{
    using Core;
    using Core.DbContext;
    using System.Collections.Generic;
    using System.Linq;

    public class Utils
    {
        public static void MergeChanges(object destination, object source)
        {
            if(destination == null || source == null) return;

            var srcProps = source.GetType()
                .GetProperties()
                .ToDictionary(x => x.Name);

            foreach (var prop in destination.GetType().GetProperties())
            {
                if (srcProps.ContainsKey(prop.Name))
                {
                    if (srcProps[prop.Name].PropertyType == prop.PropertyType)
                    {
                        var value = srcProps[prop.Name].GetGetMethod().Invoke(source, null);

                        prop.GetSetMethod().Invoke(destination, new[] { value });
                    }
                }
            }
        }

        /// <summary>
        /// Сформировать список объектов - файлов из строки
        /// </summary>
        /// <param name="hashWithDelimeters">Строка с разделителями, содержащая хэши файлов</param>
        /// <param name="delimeter">Разделитель</param>
        /// <returns>Список объектов - файлов</returns>
        public static List<DataFileResult> GetFilesListFromRow(string hashWithDelimeters, char delimeter)
        {
            if (string.IsNullOrEmpty(hashWithDelimeters))
            {
                return null;
            }

            var result = new List<DataFileResult>();
            var hashes = hashWithDelimeters.Split(delimeter);

            using (var _db = new GrantDbContext())
            {
                var data = _db.FilesInfo.Where(x => hashes.Contains(x.Guid)).ToList();
                result.AddRange(
                    data
                    .Select(x =>
                        new DataFileResult(x.VirtualPath, x.FileName + "  " + (x.EditDate.ToString("dd.MM.yyyy HH:mm")), x.Guid, x.EditDate))
                        .OrderBy(x=>x.EditDate)
                        .ToList());
            }

            return result;
        }
    }
}