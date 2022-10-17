namespace Grant.WebApi.Request
{
    using System;
    using System.Collections.Generic;
    using Core.Enum;
    //using Newtonsoft.Json;

    public class PersonalInfoData
    {

        public Sex Sex { get; set; }

        /// <summary>
        /// Гражданство
        /// </summary>
        ////[JsonProperty("citizenship")]
        public Citizenship Citizenship { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        public bool IsLiveAddressSame { get; set; }

        #region Паспортные данные

        /// <summary>
        /// Дата рождения 
        /// </summary>
        //[JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Место рождения
        /// </summary>
        //[JsonProperty("birthplace")]
        public string Birthplace { get; set; }

        /// <summary>
        /// Серия паспорта
        /// </summary>
        //[JsonProperty("passportSeries")]
        public string PassportSeries { get; set; }

        /// <summary>
        /// Номер паспорта
        /// </summary>
        //[JsonProperty("passportNumber")]
        public string PassportNumber { get; set; }

        /// <summary>
        /// Дата выдачи паспорта
        /// </summary>
        //[JsonProperty("passportIssueDate")]
        public DateTime? PassportIssueDate { get; set; }

        /// <summary>
        /// Кем выдан пасорт
        /// </summary>
        //[JsonProperty("passportIssuedBy")]
        public string PassportIssuedBy { get; set; }

        /// <summary>
        /// Код подразделения, выдавшего паспорт
        /// </summary>
        //[JsonProperty("passportIssuedByCode")]
        public string PassportIssuedByCode { get; set; }


        public bool Agreement { get; set; }

        //TODO: обычно несколько сканов
        /// <summary>
        /// Скан паспорта
        /// </summary>
        public string PassportScan { get; set; }


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

        public ICollection<string> PassportScans { get; set; }

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