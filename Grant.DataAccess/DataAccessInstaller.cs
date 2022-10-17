namespace Grant.DataAccess
{
    using Grant.Install;
    using Grant.Core.DbContext;
    using LightInject;
    using Grant.DataAccess;

    class DataAccessInstaller : BaseInstaller
    {
        public override void Install()
        {
            Container.Register<ISession,Session>(new PerRequestLifeTime());
            Container.Register(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
