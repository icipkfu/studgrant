using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grant.Core;
using Grant.Core.DbContext;
using Grant.Core.Entities;
using System.Data.Entity;
using Grant.Core.Enum;

namespace Grant.Services.DomainService
{
    public class RecordBookDataService : IRecordBookDataService
    {
        IStudentService _studentDomainService;
        public RecordBookDataService(IStudentService studentDomainService)
        {
            this._studentDomainService = studentDomainService;
        }
        public async Task<DataResult> Delete(Student student, string hash)
        {
            var recordBookStringUpdateResult = await this.UpdateRecordBookString(student, hash);
            if (!recordBookStringUpdateResult.Success)
            {
                return DataResult.Failure("Не удалось удалить запись");
            }
            using (var dbContext = new GrantDbContext())
            {
                var fileInfoToDelete = await dbContext.FilesInfo.Where(x => x.Guid.Equals(hash)).FirstOrDefaultAsync();
                dbContext.FilesInfo.Remove(fileInfoToDelete);

                return await dbContext.SaveChangesAsync() > 0 
                    ? DataResult.Ok() 
                    : DataResult.Failure("Не удалось удалить запись");
            }
        }

        /// <summary>
        /// Удаляем запись о файле в сущности, путем вырезания из строки хеша
        /// </summary>
        /// <param name="info"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private Task<DataResult> UpdateRecordBookString(Student student, string hash)
        {
            if (student.RecordBookFiles.EndsWith(hash))
            {
                student.RecordBookFiles = student.RecordBookFiles.Replace(hash, string.Empty);
            }
            else
            {
                student.RecordBookFiles = student.RecordBookFiles.Replace(hash + "|", string.Empty);
            }

            if (string.IsNullOrEmpty(student.RecordBookFiles) || student.RecordBookFiles.Replace("|", "").Length == 0) 
            {
                student.StudentBookState = ValidationState.NotFilled;
            }
            else
            {
                student.StudentBookState = ValidationState.Unknown;
            }

            student.RecordBookEditDate = DateTime.Now;
               
            return this._studentDomainService.Update(student);
        }
    }
}
