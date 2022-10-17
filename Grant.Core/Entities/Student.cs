using System.ComponentModel.DataAnnotations.Schema;

namespace Grant.Core.Entities
{
    using System.Collections.Generic;
    using Base;
    using Enum;
    using Newtonsoft.Json;
    using System;

    public class Student : BaseEntity
    {
        public string UserIdentityId { get; set; }

        [JsonIgnore]
        public DataFile Avatar { get; set; }

        public University University { get; set; }

        public long? UniversityId { get; set; }

        [JsonIgnore]
        public Portfolio Portfolio { get; set; }

        [JsonIgnore]
        public PersonalInfo PersonalInfo { get; set; }

        public long PersonalInfoId { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string Departament { get; set; }

        public Sex Sex { get; set; }

        public string AboutSelf { get; set; }

        public string SocialNetworkLinks { get; set; }

        [JsonIgnore]
        public Role? Role { get; set; }

        [JsonIgnore]
        public ICollection<Achievement> Achievements;

        [InverseProperty("Student")]
        [JsonIgnore]
        public virtual ICollection<GrantStudent> GrantStudents { get; set; }

        public string ImageFile { get; set; }

        public string RecordBookFiles { get; set; }

        public string IncomeFiles { get; set; }

        public int? Income { get; set; }

        public ValidationState PassportState { get; set; }

        public ValidationState StudentBookState { get; set; }

        public ValidationState IncomeState { get; set; }

        public string PassValidationComment { get; set; }

        public string BookValidationComment { get; set; }

        public string IncomeValidationComment { get; set; }

        [NotMapped]
        public string ImageLink { get; set; }

        [NotMapped]
        public string thumb { get; set; }

        [NotMapped]
        public string Fio { get; set; }

        public int Score { get; set; }

        [NotMapped]
        public bool IsPassportValid { get; set; }

        [NotMapped]
        public bool IsStudentBookValid { get; set; }

        [NotMapped]
        public string IsPassDataFilled { get; set; }

        [NotMapped]
        public string UniversityName { get; set; }

        [NotMapped]
        public string EditDateTime { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string Phone { get; set; }

        public GrantFileInfo AvatarFile { get; set; }

        public long? AvatarFileId { get; set; }

        [NotMapped]
        public Citizenship Citizenship { get; set; }

        public DateTime ProfileEditDate { get; set; }

        public DateTime PersonalDataEditDate { get; set; }

        public DateTime RecordBookEditDate { get; set; }

        public DateTime IncomeEditDate { get; set; }
    }
}