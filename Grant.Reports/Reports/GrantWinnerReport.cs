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
using Stimulsoft.Report;

namespace Grant.Reports.Reports
{
    public class GrantWinnerReport : BaseStimulReport
    {

        public long GrantId { get; set; }

        public Citizenship Citizenship { get; set; }

        public string Town { get; set; }

        public long UniversityId { get; set; }

        public bool ByUniversity { get; set; }

        public int Count { get; set; }

        /// <summary>
        /// Признак того, что отчет формируется по дополнительным участникам
        /// </summary>
        public bool AdditionalStudents { get; set; }

        public bool AllWinners { get; set; }

        public bool? OnlyNewWinners { get; set; }

        public override Stream GetTemplate()
        {
            return new MemoryStream(Resources.GrantWinnerReport);
        }

        public override string GetFileName()
        {
            return String.Format("{0}.pdf", Town);
        }

        public class GrantStudentInfo
        {
            public string StudentName { get; set; }
            public string FamilyName { get; set; }
            public string Patronymic { get; set; }
            public string PassportSerialNo { get; set; }
            public string PassportNo { get; set; }
            public string PassportDate { get; set; }
            public string TabNum { get; set; }

            public DateTime? PassportIssueDate { get; set; }
        }

        public override void PrepareReport()
        {
            var db = new GrantDbContext();

            IQueryable<GrantStudent> studentsQuery;


            var oldWinners = 
             db.GrantStudents
             .Include(x => x.Student)
             .Where(x => x.GrantId != GrantId)
             .Where(x => x.IsWinner || x.IsAdditionalWinner)
             .Select(x => x.StudentId).ToArray();

            if (AllWinners)
            {
                studentsQuery = db.GrantStudents
                      .Include(x => x.Student)
                      .Include(x => x.Student.PersonalInfo)
                      .Include(x => x.University)
                      .Where(gs => gs.GrantId == GrantId) 
                      .WhereIf(!ByUniversity, gs=> gs.University.Town == Town)
                      .WhereIf(ByUniversity, gs => gs.UniversityId == UniversityId)
                      .Where(gs => gs.IsWinner || gs.IsAdditionalWinner);
            }
            else
            {
                if (!AdditionalStudents)
                {
                    studentsQuery = db.GrantStudents
                        .Include(x => x.Student)
                        .Include(x => x.Student.PersonalInfo)
                        .Include(x => x.University) 
                        .Where(gs => gs.GrantId == GrantId)
                        .WhereIf(!ByUniversity, gs => gs.University.Town == Town)
                        .WhereIf(ByUniversity, gs => gs.UniversityId == UniversityId)
                        .Where(gs => gs.IsWinner);
                }
                else
                {
                    studentsQuery = db.GrantStudents
                        .Include(x => x.Student)
                        .Include(x => x.Student.PersonalInfo)
                        .Include(x => x.University)
                        .Where(gs => gs.GrantId == GrantId)
                        .WhereIf(!ByUniversity, gs => gs.University.Town == Town)
                        .WhereIf(ByUniversity, gs => gs.UniversityId == UniversityId)
                        .Where(gs => gs.IsAdditionalWinner);
                }
            }

          

            var students = studentsQuery
                .Where(gs => gs.Student.PersonalInfo.Citizenship == Citizenship)
                .WhereIf(OnlyNewWinners.HasValue && OnlyNewWinners.Value, x => !oldWinners.Contains(x.StudentId))
                .WhereIf(OnlyNewWinners.HasValue && !OnlyNewWinners.Value, x => oldWinners.Contains(x.StudentId))  
                .Select(gs => new GrantStudentInfo
                {
                    StudentName = gs.Student.LastName,
                    FamilyName = gs.Student.Name,
                    Patronymic = gs.Student.Patronymic, 
                    PassportIssueDate = gs.Student.PersonalInfo.PassportIssueDate,
                    PassportNo = gs.Student.PersonalInfo.PassportNumber,
                    PassportSerialNo = gs.Student.PersonalInfo.PassportSeries
                })
                .OrderBy(x => x.FamilyName).ThenBy(x=>x.StudentName).ThenBy(x=>x.Patronymic)
                .ToList();


            foreach (var rec in students)
            {
                rec.PassportDate = rec.PassportIssueDate.HasValue && Citizenship == Citizenship.Rf ? rec.PassportIssueDate.Value.ToString("dd.MM.yyyy") : "";
                rec.PassportNo = Citizenship == Citizenship.Rf ? rec.PassportNo : "";
                rec.PassportSerialNo = Citizenship == Citizenship.Rf ? rec.PassportSerialNo : "";
            }

            Count = students.Count();

            Report.RegData("GrantStudents", students);
        }

        public override async Task PrepareReportAsync()
        {
            var db = new GrantDbContext();

            IQueryable<GrantStudent> studentsQuery;

            if (!AdditionalStudents)
            {
                studentsQuery = db.GrantStudents.Include(x => x.Student)
                    .Include(x => x.Student.PersonalInfo)
                    .Include(x => x.University)
                    .Where(gs => gs.GrantId == GrantId && gs.University.Town == Town)
                    .Where(gs => gs.IsWinner);
            }
            else
            {
                studentsQuery = db.GrantStudents
                    .Include(x => x.Student)
                    .Include(x => x.Student.PersonalInfo)
                    .Include(x => x.University)
                    .Where(gs => gs.GrantId == GrantId && gs.University.Town == Town)
                    .Where(gs => gs.IsAdditionalWinner);
            }

            var students = await studentsQuery
                .Where(gs => gs.Student.PersonalInfo.Citizenship == Citizenship)
                .Select(gs => new GrantStudentInfo
                {
                    StudentName = gs.Student.LastName,
                    FamilyName = gs.Student.Name,
                    Patronymic = gs.Student.Patronymic,
                    PassportIssueDate = gs.Student.PersonalInfo.PassportIssueDate,
                    PassportNo = gs.Student.PersonalInfo.PassportNumber,
                    PassportSerialNo = gs.Student.PersonalInfo.PassportSeries
                })
                .OrderBy(x => x.FamilyName).ThenBy(x => x.StudentName).ThenBy(x => x.Patronymic)
                .ToListAsync();


            foreach (var rec in students)
            {
                rec.PassportDate = rec.PassportIssueDate.HasValue ? rec.PassportIssueDate.Value.ToString("dd.MM.yyyy") : "";
            }

            Report.RegData("GrantStudents", students);
        }
    }
}
