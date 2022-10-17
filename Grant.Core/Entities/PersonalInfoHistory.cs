using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Grant.Core.Entities.Base;
using Grant.Core.Enum;

namespace Grant.Core.Entities
{
    public class PersonalInfoHistory : BaseEntity
    {
        public void MakeHistory(PersonalInfo persInfo, long studentId, DateTime editDate)
        {
            this.EditTime = editDate;
            this.StudentId = studentId;
            this.Sex = persInfo.Sex;
            this.IsLiveAddressSame = persInfo.IsLiveAddressSame;
            this.Citizenship = persInfo.Citizenship;
            this.Phone = persInfo.Phone;
            this.Inn = persInfo.Inn;
            this.Birthday = persInfo.Birthday;
            this.Birthplace = persInfo.Birthplace;
            this.PassportSeries = persInfo.PassportSeries;
            this.PassportNumber = persInfo.PassportNumber;
            this.PassportIssueDate = persInfo.PassportIssueDate;
            this.PassportIssuedBy = persInfo.PassportIssuedBy;
            this.PassportIssuedByCode = persInfo.PassportIssuedByCode;
            this.PassportScan = persInfo.PassportScan;
            this.Agreement = persInfo.Agreement;
            this.PassportPage1 = persInfo.PassportPage1;
            this.PassportPage2 = persInfo.PassportPage2;
            this.PassportPage3 = persInfo.PassportPage3;
            this.PassportPage4 = persInfo.PassportPage4;
            this.PassportPage5 = persInfo.PassportPage5;
            this.PassportPage6 = persInfo.PassportPage6;
            this.PassportPage7 = persInfo.PassportPage7;
            this.PassportPage8 = persInfo.PassportPage8;
            this.PassportPage9 = persInfo.PassportPage9;
            this.PassportPage10 = persInfo.PassportPage10;
            this.PassportPage1Id = persInfo.PassportPage1Id;
            this.PassportPage2Id = persInfo.PassportPage2Id;
            this.PassportPage3Id = persInfo.PassportPage3Id;
            this.PassportPage4Id = persInfo.PassportPage4Id;
            this.PassportPage5Id = persInfo.PassportPage5Id;
            this.PassportPage6Id = persInfo.PassportPage6Id;
            this.PassportPage7Id = persInfo.PassportPage7Id;
            this.PassportPage8Id = persInfo.PassportPage8Id;
            this.PassportPage9Id = persInfo.PassportPage9Id;
            this.PassportPage10Id = persInfo.PassportPage10Id;
            this.RegistrationRepublic = persInfo.RegistrationRepublic;
            this.RegistrationzZone = persInfo.RegistrationzZone;
            this.RegistrationIndex = persInfo.RegistrationIndex;
            this.RegistrationCity = persInfo.RegistrationCity;
            this.RegistrationPlace = persInfo.RegistrationPlace;
            this.RegistrationStreet = persInfo.RegistrationStreet;
            this.RegistrationHouse = persInfo.RegistrationHouse;
            this.RegistrationHousing = persInfo.RegistrationHousing;
            this.RegistrationFlat = persInfo.RegistrationFlat;
            this.LiveRepublic = persInfo.LiveRepublic;
            this.LiveZone = persInfo.LiveZone;
            this.LiveIndex = persInfo.LiveIndex;
            this.LiveCity = persInfo.LiveCity;
            this.LivePlace = persInfo.LivePlace;
            this.LiveStreet = persInfo.LiveStreet;
            this.LiveHouse = persInfo.LiveHouse;
            this.LiveHousing = persInfo.LiveHousing;
            this.LiveFlat = persInfo.LiveFlat;
        }

        public PersonalInfo LoadHistory(PersonalInfo persInfo)
        {
            persInfo.Sex = this.Sex;
            persInfo.IsLiveAddressSame = this.IsLiveAddressSame;
            persInfo.Citizenship = this.Citizenship;
            persInfo.Phone = this.Phone;
            persInfo.Inn = this.Inn;
            persInfo.Birthday = this.Birthday;
            persInfo.Birthplace = this.Birthplace;
            persInfo.PassportSeries = this.PassportSeries;
            persInfo.PassportNumber = this.PassportNumber;
            persInfo.PassportIssueDate = this.PassportIssueDate;
            persInfo.PassportIssuedBy = this.PassportIssuedBy;
            persInfo.PassportIssuedByCode = this.PassportIssuedByCode;
            persInfo.PassportScan = this.PassportScan;
            persInfo.Agreement = this.Agreement;
            persInfo.PassportPage1 = this.PassportPage1;
            persInfo.PassportPage2 = this.PassportPage2;
            persInfo.PassportPage3 = this.PassportPage3;
            persInfo.PassportPage4 = this.PassportPage4;
            persInfo.PassportPage5 = this.PassportPage5;
            persInfo.PassportPage6 = this.PassportPage6;
            persInfo.PassportPage7 = this.PassportPage7;
            persInfo.PassportPage8 = this.PassportPage8;
            persInfo.PassportPage9 = this.PassportPage9;
            persInfo.PassportPage10 = this.PassportPage10;
            persInfo.PassportPage1Id = this.PassportPage1Id;
            persInfo.PassportPage2Id = this.PassportPage2Id;
            persInfo.PassportPage3Id = this.PassportPage3Id;
            persInfo.PassportPage4Id = this.PassportPage4Id;
            persInfo.PassportPage5Id = this.PassportPage5Id;
            persInfo.PassportPage6Id = this.PassportPage6Id;
            persInfo.PassportPage7Id = this.PassportPage7Id;
            persInfo.PassportPage8Id = this.PassportPage8Id;
            persInfo.PassportPage9Id = this.PassportPage9Id;
            persInfo.PassportPage10Id = this.PassportPage10Id;
            persInfo.RegistrationRepublic = this.RegistrationRepublic;
            persInfo.RegistrationzZone = this.RegistrationzZone;
            persInfo.RegistrationIndex = this.RegistrationIndex;
            persInfo.RegistrationCity = this.RegistrationCity;
            persInfo.RegistrationPlace = this.RegistrationPlace;
            persInfo.RegistrationStreet = this.RegistrationStreet;
            persInfo.RegistrationHouse = this.RegistrationHouse;
            persInfo.RegistrationHousing = this.RegistrationHousing;
            persInfo.RegistrationFlat = this.RegistrationFlat;
            persInfo.LiveRepublic = this.LiveRepublic;
            persInfo.LiveZone = this.LiveZone;
            persInfo.LiveIndex = this.LiveIndex;
            persInfo.LiveCity = this.LiveCity;
            persInfo.LivePlace = this.LivePlace;
            persInfo.LiveStreet = this.LiveStreet;
            persInfo.LiveHouse = this.LiveHouse;
            persInfo.LiveHousing = this.LiveHousing;
            persInfo.LiveFlat = this.LiveFlat;

            return persInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EditTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long StudentId { get; set; }

        public Sex Sex { get; set; }

        public Boolean IsLiveAddressSame { get; set; }

        /// <summary>
        /// Гражданство
        /// </summary>
        public Citizenship Citizenship { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        #region Паспортные данные

        /// <summary>
        /// Дата рождения 
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Место рождения
        /// </summary>
        public string Birthplace { get; set; }

        /// <summary>
        /// Серия паспорта
        /// </summary>
        public string PassportSeries { get; set; }

        /// <summary>
        /// Номер паспорта
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// Дата выдачи паспорта
        /// </summary>
        public DateTime? PassportIssueDate { get; set; }

        /// <summary>
        /// Кем выдан пасорт
        /// </summary>
        public string PassportIssuedBy { get; set; }

        /// <summary>
        /// Код подразделения, выдавшего паспорт
        /// </summary>
        public string PassportIssuedByCode { get; set; }

        /// <summary>
        /// Сканы страниц паспорта
        /// </summary>
        // public ICollection<GrantFileInfo> PassportScan { get; set; }

        //[NotMapped]
        public ICollection<PersonalInfoFile> PassportScans { get; set; }

        public string PassportScan { get; set; }

        public bool Agreement { get; set; }

        [NotMapped]
        public ICollection<Core.DataFileResult> PassportScanLinks { get; set; }

        public GrantFileInfo PassportPage1 { get; set; }
        public GrantFileInfo PassportPage2 { get; set; }
        public GrantFileInfo PassportPage3 { get; set; }
        public GrantFileInfo PassportPage4 { get; set; }
        public GrantFileInfo PassportPage5 { get; set; }
        public GrantFileInfo PassportPage6 { get; set; }
        public GrantFileInfo PassportPage7 { get; set; }

        public GrantFileInfo PassportPage8 { get; set; }

        public GrantFileInfo PassportPage9 { get; set; }

        public GrantFileInfo PassportPage10 { get; set; }

        public long? PassportPage1Id { get; set; }
        public long? PassportPage2Id { get; set; }
        public long? PassportPage3Id { get; set; }
        public long? PassportPage4Id { get; set; }
        public long? PassportPage5Id { get; set; }
        public long? PassportPage6Id { get; set; }
        public long? PassportPage7Id { get; set; }

        public long? PassportPage8Id { get; set; }

        public long? PassportPage9Id { get; set; }

        public long? PassportPage10Id { get; set; }

        #endregion Паспортные данные

        #region Прописка

        /// <summary>
        /// Республика
        /// </summary>
        public string RegistrationRepublic { get; set; }

        /// <summary>
        /// Район
        /// </summary>
        public string RegistrationzZone { get; set; }

        /// <summary>
        /// Индекс места регистрации
        /// </summary>
        public string RegistrationIndex { get; set; }

        /// <summary>
        /// Город места регистрации
        /// </summary>
        public string RegistrationCity { get; set; }

        /// <summary>
        /// Населенный пункт
        /// </summary>
        public string RegistrationPlace { get; set; }

        /// <summary>
        /// Улица места регистрации
        /// </summary>
        public string RegistrationStreet { get; set; }

        /// <summary>
        /// Дом места регистрации
        /// </summary>
        public string RegistrationHouse { get; set; }

        /// <summary>
        /// Корпус места регистрации (не обязательно)
        /// </summary>
        public string RegistrationHousing { get; set; }

        /// <summary>
        /// Квартира места регистрации (не обязательно)
        /// </summary>
        public string RegistrationFlat { get; set; }

        #endregion Прописка

        #region Проживание

        /// <summary>
        /// Республика
        /// </summary>
        public string LiveRepublic { get; set; }

        /// <summary>
        /// Район
        /// </summary>
        public string LiveZone { get; set; }

        /// <summary>
        /// Индекс места регистрации
        /// </summary>
        public string LiveIndex { get; set; }

        /// <summary>
        /// Город места регистрации
        /// </summary>
        public string LiveCity { get; set; }

        /// <summary>
        /// Населенный пункт
        /// </summary>
        public string LivePlace { get; set; }

        /// <summary>
        /// Улица места регистрации
        /// </summary>
        public string LiveStreet { get; set; }

        /// <summary>
        /// Дом места регистрации
        /// </summary>
        public string LiveHouse { get; set; }

        /// <summary>
        /// Корпус места регистрации (не обязательно)
        /// </summary>
        public string LiveHousing { get; set; }

        /// <summary>
        /// Квартира места регистрации (не обязательно)
        /// </summary>
        public string LiveFlat { get; set; }

        #endregion Проживание
    }
}
