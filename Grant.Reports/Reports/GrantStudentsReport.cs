using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grant.Core.Context;
using Grant.Core.DbContext;
using Grant.Core.Entities;
using Grant.Core.Enum;
using Grant.Core.Migrations;
using Grant.Reports.Properties;
using Grant.Reports.Reports.Base;
using Grant.Utils.Extensions;

namespace Grant.Reports.Reports
{
    public class GrantStudentsReport : BaseStimulReport
    {
        private GrantDbContext db = new GrantDbContext();

        public long GrantId { get; set; }

        public long UniversityId { get; set; }

        public bool AllAdditional { get; set; }

        /// <summary>
        /// Признак того, что отчет формируется по дополнительным участникам
        /// </summary>
        public bool AdditionalStudents { get; set; }

        public bool? OnlyNewWinners { get; set; }

        public override Stream GetTemplate()
        {
            return new MemoryStream(Resources.GrantStudentsReport);
        }

        public override string GetFileName()
        {
            if (AllAdditional)
            {
                return "Отчет_по_дополнительным_победителям.doc";
            }
            else
            {
                return "GrantStudentsReport.pdf";
            }
        }

        public class GrantStudentInfo
        {
            public string FIO { get; set; }
            public string Department { get; set; }
            public string AboutSelf { get; set; }
            public List<AchievementSubject> Achievements { get; set; }
            public int Score { get; set; }
            public string Phone { get; set; }
        }

        public override async Task PrepareReportAsync()
        {
            var oldWinners = await
           db.GrantStudents
           .Include(x => x.Student)
           .Where(x => x.GrantId != GrantId)
           .Where(x => x.IsWinner || x.IsAdditionalWinner)
           .Select(x => x.StudentId).ToArrayAsync();

            IQueryable<GrantStudent> studentsQuery;


            var grant = await db.Grants.SingleOrDefaultAsync(x => x.Id == GrantId);

            var id = ApplicationContext.Current.CurUserId();
            var stud = await db.Students.SingleOrDefaultAsync(x => x.UserIdentityId == id);

            var fio = "";

            if (stud != null)
            {
                fio = string.Format("{0} {1} {2}", stud.Name, stud.LastName, stud.Patronymic);
            }

            Report["GrantName"] = grant.Name;
            Report["ReportDate"] = DateTime.Now.Date.ToString("dd.MM.yyyy");
            Report["ReportTime"] = DateTime.Now.ToString("HH:mm:ss");
            Report["ResponsibleFIO"] = fio;
            Report["ResponsiblePhone"] = stud.Phone;

            if (!AllAdditional)
            {
                var university = await db.Universities.SingleOrDefaultAsync(x => x.Id == UniversityId);

               
                var quota =
                    await
                        db.GrantQuotas.SingleOrDefaultAsync(x => x.UniversityId == UniversityId && x.GrantId == GrantId);

                

                Report["UniversityName"] = university.Name;
                Report["UniversityAddress"] = university.Address;
                Report["UniversityQuote"] = quota != null ? quota.Quota : 0;

                // TODO: хардкод, нужно найти эти данные
                Report["RectorFIO"] = "";
                Report["ReceptionPhone"] = "";
                Report["ResponsibleContacts"] = fio;


                if (!AdditionalStudents)
                {
                    studentsQuery = db.GrantStudents
                        .Where(gs => gs.GrantId == GrantId && gs.UniversityId == UniversityId)
                        .Where(gs => gs.IsWinner);
                }
                else
                {
                    studentsQuery = db.GrantStudents
                        .Where(gs => gs.GrantId == GrantId && gs.UniversityId == UniversityId)
                        .Where(gs => gs.IsAdditionalWinner);
                }

            }
            else
            {
                studentsQuery = db.GrantStudents
                       .Where(gs => gs.GrantId == GrantId)
                       .Where(gs => gs.IsAdditionalWinner);
            }

            var students = await studentsQuery
                .WhereIf(OnlyNewWinners.HasValue && OnlyNewWinners.Value, x => !oldWinners.Contains(x.StudentId))
                .WhereIf(OnlyNewWinners.HasValue && !OnlyNewWinners.Value, x => oldWinners.Contains(x.StudentId)) 
                .Select(gs => new GrantStudentInfo
                {
                    FIO = gs.Student.Name + " " + gs.Student.LastName + " " + gs.Student.Patronymic, 
                    Department = gs.Student.Departament, 
                    Achievements = db.Achievements.Where(a => a.Student == gs.Student).Select(a => a.Subject).Distinct().ToList(),
                    Score =  gs.Student.Score,
                    Phone = "+7" + gs.Student.Phone
                })
                .OrderBy(x => x.FIO)
                .ToListAsync();

            foreach (var student in students)
            {
                student.AboutSelf = string.Join(", ", student.Achievements.Select(x => x.GetAttribute<DisplayAttribute>().Name).OrderBy(x => x));
            }

            Report.RegData("GrantStudents", students);
        }
    }
}
