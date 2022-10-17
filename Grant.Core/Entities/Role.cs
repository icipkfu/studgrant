
namespace Grant.Core.Entities
{
    //[Table("ROLES", Schema = "GRANT")]
    public enum Role
    {
        AnonimusUser = 1,
        RegistredUser = 2,
        UniversityCurator =3,
        ZoneModerator = 4,
        Administrator = 5,
        GrantAdministrator = 6
    }
}