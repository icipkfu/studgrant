using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetDBF;
using Grant.Core.DbContext;
using Grant.Core.Entities;
using Grant.Core.Enum;
using Grant.Utils.Extensions;
using Stimulsoft.Report;

namespace Grant.Reports.Reports
{

    public class DbfReportGenerator : IDbfReportGenerator
    {
        private ISession Session;

        public long[] ids;

        public DbfReportGenerator(ISession session)
        {
            Session = session;
        }


        private string  FormatPassportSeries(string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Length == 4)
            {
                return string.Format("{0} {1}", value.Substring(0, 2), value.Substring(2, 2));
            }

            return "";
        }

        private string FormatAddressRegister(PersonalInfo value)
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                value.RegistrationIndex,
                value.RegistrationRepublic,
                value.RegistrationzZone,
                value.RegistrationCity,
                value.RegistrationPlace,
                value.RegistrationStreet,
                value.RegistrationHouse,
                value.RegistrationHousing,
                value.RegistrationFlat);
        }

        private string FormatAddressLive(PersonalInfo value)
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                value.LiveIndex,
                value.LiveRepublic,
                value.LiveZone,
                value.LiveCity,
                value.LivePlace,
                value.LiveStreet,
                value.LiveHouse,
                value.LiveHousing,
                value.LiveFlat);
        }

        public MemoryStream GetDbfReport(long grantId, string town, long universityId, bool additional, bool byUniversity, bool? onlyNewWinners)
        {
            var context = new GrantDbContext();


            var oldWinners =
            context.GrantStudents
                .Include(x => x.Student)
                .Where(x => x.GrantId != grantId)
                .Where(x => x.IsWinner || x.IsAdditionalWinner)
                .Select(x => x.StudentId).ToArray();

            var data = context.GrantStudents
                .Include(x => x.Student)
                .Include(x=>x.Student.PersonalInfo)
                .Where(x => x.GrantId == grantId)
                .WhereIf(byUniversity, x=> x.UniversityId == universityId)
                .WhereIf(!byUniversity,x => x.University.Town == town)
                .WhereIf(onlyNewWinners.HasValue && onlyNewWinners.Value, x=> !oldWinners.Contains(x.StudentId))
                .WhereIf(onlyNewWinners.HasValue && !onlyNewWinners.Value, x => oldWinners.Contains(x.StudentId))  
                .Where(x=>x.Student.PersonalInfo.Citizenship == Citizenship.Rf)
                .WhereIf(additional,  x=>x.IsAdditionalWinner)
                .WhereIf(!additional, x=>x.IsWinner)
                .Select(x=>x.Student)
                .Include(x=>x.PersonalInfo)
                .Include(x=>x.University)
                .Include(x=>x.University.BankFilial)
                .ToList();


            if (ids != null && ids.Any())
            {
                data = context.GrantStudents
                .Include(x => x.Student)
                .Include(x => x.Student.PersonalInfo)
                .Where(x => ids.Contains(x.StudentId))
                .Where(x => x.Student.PersonalInfo.Citizenship == Citizenship.Rf)
                .Select(x => x.Student).Include(x => x.PersonalInfo)
                .ToList();
            }

            if (!data.Any())
                return new MemoryStream();

            var writer = new DBFWriter();
            var field1 = new DBFField("SUMMA", NativeDbType.Numeric, 12,2);
            var field2 = new DBFField("CLNT_FNAME", NativeDbType.Char, 35);
            var field3 = new DBFField("CLNT_INAME", NativeDbType.Char, 20);
            var field4 = new DBFField("CLNT_ONAME", NativeDbType.Char, 30);
            var field5 = new DBFField("CLNT_SPAS", NativeDbType.Char, 10);
            var field6 = new DBFField("CLNT_NPAS", NativeDbType.Char, 35);
            var field7 = new DBFField("CLNT_DPAS", NativeDbType.Date);
            var field8 = new DBFField("CLNT_PPAS", NativeDbType.Char, 50);
            var field9 = new DBFField("CLNT_ADR", NativeDbType.Char, 100);
            var field10 = new DBFField("BIRTHDAY", NativeDbType.Date);
            var field11 = new DBFField("TAB_N", NativeDbType.Char, 20);
            var field12 = new DBFField("ACC", NativeDbType.Char, 25);
            var field13 = new DBFField("WORD", NativeDbType.Char, 32);
            var field14 = new DBFField("PHONE", NativeDbType.Char, 32);
            var field15 = new DBFField("INN", NativeDbType.Char, 20);
            var field16 = new DBFField("FIL", NativeDbType.Char, 10);
            var field17 = new DBFField("TRANSLIT", NativeDbType.Char, 19);
            var field18 = new DBFField("ADD_INFO", NativeDbType.Char, 32);
            var field19 = new DBFField("BIRTHPLACE", NativeDbType.Char, 32);
            var field20 = new DBFField("PHONE_M", NativeDbType.Char, 32);
            var field21 = new DBFField("PHONE_H", NativeDbType.Char, 32);
            var field22 = new DBFField("PHONE_F", NativeDbType.Char, 32);
            var field23 = new DBFField("CLNT_CPPAS", NativeDbType.Char, 10);
            var field24 = new DBFField("CLNT_PADR", NativeDbType.Char, 100);
            var field25 = new DBFField("DOC_TYPE", NativeDbType.Char, 10);
            var field26 = new DBFField("CLNT_EMAIL", NativeDbType.Char, 100);


            writer.CharEncoding = Encoding.GetEncoding(866);


            writer.Fields = new[] {  field1,
                                     field2,
                                     field3,
                                     field4,
                                     field5,
                                     field6,
                                     field7,
                                     field8,
                                     field9,
                                     field10,
                                     field11,
                                     field12,
                                     field13,
                                     field14,
                                     field15,
                                     field16,
                                     field17,
                                     field18,
                                     field19,
                                     field20,
                                     field21,
                                     field22,
                                     field23,
                                     field24,
                                     field25,
                                     field26};


            
            foreach (var stud in data)
            {
                try
                {
                    
                    writer.AddRecord(
                        0,
                        stud.Name,
                        stud.LastName,
                        stud.Patronymic,
                        FormatPassportSeries(stud.PersonalInfo.PassportSeries),
                        stud.PersonalInfo.PassportNumber,
                        stud.PersonalInfo.PassportIssueDate.Value,
                        stud.PersonalInfo.PassportIssuedBy,
                        FormatAddressRegister(stud.PersonalInfo),
                        stud.PersonalInfo.Birthday,
                        null,
                        null,
                        null,
                        stud.Phone,
                        stud.PersonalInfo.Inn,
                        stud.UniversityId.HasValue && stud.University.BankFilialId.HasValue? stud.University.BankFilial.Code : null,
                        null,
                        null,
                        stud.PersonalInfo.Birthplace,
                        stud.Phone,
                        null,
                        null,
                        stud.PersonalInfo.PassportIssuedByCode,
                        FormatAddressLive(stud.PersonalInfo),
                        "21",
                        null);
                }
                catch (Exception ex)
                {
                    throw;
                }
              
            }

            var result = new MemoryStream();

            writer.Write(result);

            return result;
        }

        public async Task<MemoryStream> GetDbfReportAsync(long grantId, string town, long universityId, bool additional, bool byUniversity, bool? onlyNewWinners)
        {

            var oldWinners = await
             Session.CurrentContext().GrantStudents
             .Include(x => x.Student)
             .Where(x => x.GrantId != grantId)
             .Where(x => x.IsWinner || x.IsAdditionalWinner)
             .Select(x => x.StudentId).ToArrayAsync();

            var data = await Session.CurrentContext().GrantStudents
                .Include(x => x.Student)
                .Include(x => x.Student.PersonalInfo)
                .Where(x => x.GrantId == grantId)
                .WhereIf(byUniversity, x => x.UniversityId == universityId)
                .WhereIf(!byUniversity, x => x.University.Town == town)
                .WhereIf(onlyNewWinners.HasValue && onlyNewWinners.Value, x => !oldWinners.Contains(x.StudentId))
                .WhereIf(onlyNewWinners.HasValue && !onlyNewWinners.Value, x => oldWinners.Contains(x.StudentId))  
                .Where(x => x.Student.PersonalInfo.Citizenship == Citizenship.Rf)
                .WhereIf(additional, x => x.IsAdditionalWinner)
                .WhereIf(!additional, x => x.IsWinner)
                .Select(x => x.Student).Include(x => x.PersonalInfo)
                .ToListAsync();

            if (!data.Any())
                return new MemoryStream();

            var writer = new DBFWriter();
            var field1 = new DBFField("SUMMA", NativeDbType.Numeric, 12, 2);
            var field2 = new DBFField("CLNT_FNAME", NativeDbType.Char, 35);
            var field3 = new DBFField("CLNT_INAME", NativeDbType.Char, 20);
            var field4 = new DBFField("CLNT_ONAME", NativeDbType.Char, 30);
            var field5 = new DBFField("CLNT_SPAS", NativeDbType.Char, 10);
            var field6 = new DBFField("CLNT_NPAS", NativeDbType.Char, 35);
            var field7 = new DBFField("CLNT_DPAS", NativeDbType.Date);
            var field8 = new DBFField("CLNT_PPAS", NativeDbType.Char, 50);
            var field9 = new DBFField("CLNT_ADR", NativeDbType.Char, 100);
            var field10 = new DBFField("BIRTHDAY", NativeDbType.Date);
            var field11 = new DBFField("TAB_N", NativeDbType.Char, 20);
            var field12 = new DBFField("ACC", NativeDbType.Char, 25);
            var field13 = new DBFField("WORD", NativeDbType.Char, 32);
            var field14 = new DBFField("PHONE", NativeDbType.Char, 32);
            var field15 = new DBFField("INN", NativeDbType.Char, 20);
            var field16 = new DBFField("FIL", NativeDbType.Char, 10);
            var field17 = new DBFField("TRANSLIT", NativeDbType.Char, 19);
            var field18 = new DBFField("ADD_INFO", NativeDbType.Char, 32);
            var field19 = new DBFField("BIRTHPLACE", NativeDbType.Char, 32);
            var field20 = new DBFField("PHONE_M", NativeDbType.Char, 32);
            var field21 = new DBFField("PHONE_H", NativeDbType.Char, 32);
            var field22 = new DBFField("PHONE_F", NativeDbType.Char, 32);
            var field23 = new DBFField("CLNT_CPPAS", NativeDbType.Char, 10);
            var field24 = new DBFField("CLNT_PADR", NativeDbType.Char, 100);
            var field25 = new DBFField("DOC_TYPE", NativeDbType.Char, 10);
            var field26 = new DBFField("CLNT_EMAIL", NativeDbType.Char, 100);


            writer.CharEncoding = Encoding.GetEncoding(866);


            writer.Fields = new[] {  field1,
                                     field2,
                                     field3,
                                     field4,
                                     field5,
                                     field6,
                                     field7,
                                     field8,
                                     field9,
                                     field10,
                                     field11,
                                     field12,
                                     field13,
                                     field14,
                                     field15,
                                     field16,
                                     field17,
                                     field18,
                                     field19,
                                     field20,
                                     field21,
                                     field22,
                                     field23,
                                     field24,
                                     field25,
                                     field26};



            foreach (var stud in data)
            {
                try
                {
                    writer.AddRecord(
                        0,
                        stud.Name,
                        stud.LastName,
                        stud.Patronymic,
                        FormatPassportSeries(stud.PersonalInfo.PassportSeries),
                        stud.PersonalInfo.PassportNumber,
                        stud.PersonalInfo.PassportIssueDate.Value,
                        stud.PersonalInfo.PassportIssuedBy,
                        FormatAddressRegister(stud.PersonalInfo),
                        stud.PersonalInfo.Birthday,
                        null,
                        null,
                        null,
                        stud.Phone,
                        stud.PersonalInfo.Inn,
                        null,
                        null,
                        null,
                        stud.PersonalInfo.Birthplace,
                        null,
                        null,
                        null,
                        stud.PersonalInfo.PassportIssuedByCode,
                        FormatAddressLive(stud.PersonalInfo),
                        "21",
                        null);
                }
                catch (Exception ex)
                {
                    throw;
                }

            }

            var result = new MemoryStream();

            writer.Write(result);

            return result;
        }
    }
}
