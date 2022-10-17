namespace Grant.Install
{
    using System.Collections.Generic;
    using System.Linq;
    using LightInject;
    using Grant.Core.Context;
    using Grant.Core.Install;

    public abstract class BaseInstaller : IInstaller
    {
        protected IServiceContainer Container
        {
            get
            {
                return ApplicationContext.Current.Container;
            }
        }

        string IInstaller.Name
        {
            get { return GetType().Assembly.FullName; }
        }

        IEnumerable<string> IInstaller.Dependencies
        {
            get { return GetType().Assembly.GetReferencedAssemblies().Select(x => x.FullName).ToList().AsReadOnly(); }
        }

        /// <summary>
        /// Запустить установщик
        /// </summary>
        public abstract void Install();
    }
}