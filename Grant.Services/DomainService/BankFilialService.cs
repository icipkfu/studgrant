using Grant.Core;
using Grant.Core.Entities;
using Grant.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Services.DomainService
{
    class BankFilialService : BaseDomainService<BankFilial>, IBankFilialService
    {

        private IFileManager fileManager;
        private IStudentService studentService;

        public BankFilialService(IRepository<BankFilial> repository, IFileManager fileManager, IStudentService studentService) : base(repository)
        {
            this.fileManager = fileManager;
            this.studentService = studentService;
        }


        public async override Task<DataResult> Create(BankFilial bankFilial)
        {
            if (await GetAll().AnyAsync(x => x.FilialName == bankFilial.Name))
            {
                return DataResult.Failure("Данный филиал уже зарегистрирован в системе");
            }
         
            return await base.Create(bankFilial);
        }


    }
}
