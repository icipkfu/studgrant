namespace TestDi.Impl
{
    using System;

    public class TestService : ITestService
    {
        public string SayTest()
        {
            return "Test";
        }
    }
}
