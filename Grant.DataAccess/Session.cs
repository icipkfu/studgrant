namespace Grant.DataAccess
{
    using Grant.Core.DbContext;

    class Session : ISession
    {
        private GrantDbContext _db;
        
        public GrantDbContext CurrentContext()
        {
            return (_db ?? (_db = new GrantDbContext()));
        }
    }
}
