using System;
using System.IO;
using Grant.Core.Enum;

namespace Grant.WebApi.Controllers
{
    using Grant.Services.DomainService;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Grant.Utils.Extensions;
    using Core.Services;
    using Core;

    public class RecordBookDataController : BaseController
    {
        private readonly IStudentService _studentService;
        //     private readonly IRecordBookDataService _recordBookDataService;
        private ITaskService _taskService;

        public RecordBookDataController()
        {
            this._studentService = Container.Get<IStudentService>();
            this._taskService = Container.Get<ITaskService>();
        }

        [HttpGet]
        [Route("api/recordBook/{id}")]
        public async Task<IHttpActionResult> Get(long id)
        {
            var currentStudent = await this._studentService.GetCurrent();
            var recordBookFiles = Utils.GetFilesListFromRow(currentStudent.RecordBookFiles, '|');

            return this.Ok(recordBookFiles);
        }

        [HttpGet]
        [Route("api/recordBook/getbystudent/{id}")]
        public async Task<IHttpActionResult> GetbyStudent(long id)
        {
            if (id == 0)
            {
                id = (await _studentService.GetCurrent()).Id;
            }

            var currentStudent = await _studentService.Get(id);
            var recordBookFiles = Utils.GetFilesListFromRow(currentStudent.RecordBookFiles, '|');

            return this.Ok(recordBookFiles);
        }
        

        [HttpGet]
        [Route("api/recordBook/delete/{hash}")]
        public async Task<IHttpActionResult> Delete(string hash)
        {
            var recordBookDataService = Container.Get<IRecordBookDataService>();

            var currentStudent = await this._studentService.GetCurrent();

            var dataResult = await recordBookDataService.Delete(currentStudent, hash);
            var recordBookFiles = Utils.GetFilesListFromRow(currentStudent.RecordBookFiles, '|');

            return this.Ok(recordBookFiles);
        }

        [HttpGet]
        [Route("api/recordBook/add/{hash}")]
        public async Task<IHttpActionResult> Add(string hash)
        {
            var currentStudent = await this._studentService.GetCurrent();

            var files = currentStudent.RecordBookFiles;
            if (!string.IsNullOrEmpty(files) && files.Length > 0)
            {
                files = String.Format("{0}|{1}", files, hash);
            }
            else
            {
                files = hash;
            }

            currentStudent.RecordBookFiles = files;

            currentStudent.StudentBookState = ValidationState.Unknown;

            currentStudent.RecordBookEditDate = DateTime.Now;

            await _studentService.Update(currentStudent);

            var recordBookFiles = Utils.GetFilesListFromRow(currentStudent.RecordBookFiles, '|');

            return this.Ok(recordBookFiles);
        }

        [HttpGet]
        [Route("api/recordBook/backup123")]
        public async Task<IHttpActionResult> Backup()
        {
            DataResult result = new DataResult();
            try
            {
                var res = await _taskService.AutoBackupTask();
                result.Success = true;
                return Ok(res);
            }
            catch(Exception ex)
            {
                result.Success = false;
                return Ok($"{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}");
            }
        }
    }
}
