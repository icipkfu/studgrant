namespace Grant.Core.DbContext
{
    public interface ISession
    {
        GrantDbContext CurrentContext();
    }
}