namespace TestDi
{
    using Grant.Install;
    using Grant.Utils.Extensions;
    using TestDi.Impl;


    public class TestDiInstaller : BaseInstaller
    {
        public override void Install()
        {
            Container.RegisterTransient<ITestService, TestService>();
        }
    }
}