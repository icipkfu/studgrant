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
    using System.Web.Http.Description;
    using Grant.Core.Entities;

    public class IncomeController : BaseController
    {
        private readonly IStudentService _studentService;

        private ITaskService _taskService;

        public IncomeController()
        {
            this._studentService = Container.Get<IStudentService>();
            this._taskService = Container.Get<ITaskService>();
        }

        [HttpGet]
        [Route("api/income/{id}")]
        public async Task<IHttpActionResult> Get(long id)
        {
            var currentStudent = await this._studentService.GetCurrent();
            var incomeFiles = Utils.GetFilesListFromRow(currentStudent.IncomeFiles, '|');

            return this.Ok(incomeFiles);
        }


        [HttpPut]
        [ResponseType(typeof(Student))]
        [Route("api/income/set/{amount}")]
        public async Task<IHttpActionResult> Set(int amount)
        {
            var student = await this._studentService.GetCurrent();
            
            if(student != null)
            {
               await this._studentService.SetIncome(student.Id, amount);
            }


            return this.Ok(student);
        }

        [HttpGet]
        [Route("api/income/getbystudent/{id}")]
        public async Task<IHttpActionResult> GetbyStudent(long id)
        {
            if (id == 0)
            {
                id = (await _studentService.GetCurrent()).Id;
            }

            var currentStudent = await _studentService.Get(id);
            var incomeFiles = Utils.GetFilesListFromRow(currentStudent.IncomeFiles, '|');

            return this.Ok(incomeFiles);
        }
        

        [HttpGet]
        [Route("api/income/delete/{hash}")]
        public async Task<IHttpActionResult> Delete(string hash)
        {
            var service = Container.Get<IIncomeDataService>();

            var currentStudent = await this._studentService.GetCurrent();

            var dataResult = await service.Delete(currentStudent, hash);
            var incomeFiles = Utils.GetFilesListFromRow(currentStudent.IncomeFiles, '|');

            return this.Ok(incomeFiles);
        }

        [HttpGet]
        [Route("api/income/add/{hash}")]
        public async Task<IHttpActionResult> Add(string hash)
        {
            var currentStudent = await this._studentService.GetCurrent();

            var files = currentStudent.IncomeFiles;
            if (!string.IsNullOrEmpty(files) && files.Length > 0)
            {
                files = String.Format("{0}|{1}", files, hash);
            }
            else
            {
                files = hash;
            }

            currentStudent.IncomeFiles = files;

            currentStudent.IncomeState = ValidationState.Unknown;

            currentStudent.IncomeEditDate = DateTime.Now;

            await _studentService.Update(currentStudent);

            var incomeFiles = Utils.GetFilesListFromRow(currentStudent.IncomeFiles, '|');

            return this.Ok(incomeFiles);
        }

    }
}
