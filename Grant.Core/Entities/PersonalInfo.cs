namespace Grant.Core.Entities
{
    using System;
    using Base;
    using Enum;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Персональные данные студента
    /// </summary>
    public class PersonalInfo : BaseEntity
    {
        public void MergeChanges(PersonalInfo info)
        {
            foreach (var p in this.GetType().GetProperties())
            {
                var value = p.GetGetMethod().Invoke(info, null);
                p.GetSetMethod().Invoke(this, new[] {value});
            }
        }

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