namespace Grant.Core.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Base;

    public class User : BaseEntity
    {
        [Required] 
        public string Password { get; set; }

        [Required] 
        public string Email { get; set; }
    }
}