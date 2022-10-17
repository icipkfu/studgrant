namespace Grant.WebApi.Request
{
    using Core.Enum;
    using Core.Entities;

    public class StudentData
    {
        public long Id { get; set; }

        public string UserIdentityId { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string Departament { get; set; }

        public Sex Sex { get; set; }

        public string AboutSelf { get; set; }

        public string SocialNetworkLinks { get; set; }

        public string ImageFile { get; set; }

       // public University University { get; set; }

        public string Phone { get; set;}

        public long UniversityId { get; set; }
    }
}