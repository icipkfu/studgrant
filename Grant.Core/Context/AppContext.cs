using Microsoft.AspNet.Identity;

namespace Grant.Core.Context
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using LightInject;
    using Grant.Utils.Extensions;
    using Grant.Core.Install;
    using Grant.Core.Logging;
    using Grant.Core.Config;

    /// <summary>
    /// Имплементация контекста приложения
    /// </summary>
    public abstract class AppContext : IAppContext
    {
        private readonly IServiceContainer _container;
        private ILogManager logManager;

        /// <summary>
        /// Контейнер
        /// </summary>
        public IServiceContainer Container
        {
            get { return _container; }
        }

        public ILogManager Log
        {
            get { return (logManager ?? (logManager = Container.Get<ILogManager>())); }
        }

        /// <summary>
        /// .ctor
        /// </summary>
        protected AppContext(IServiceContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        public virtual void Start()
        {
          // Container.RegisterSingleton<IEventAggregator, EventAggregator>();
          Container.RegisterSingleton<IConfigProvider, ConfigProvider>();
          Container.RegisterSingleton<ILogManager, DefaultLogManager>();
          //  Log = Container.Get<ILogManager>();

          Log.Debug("Старт контекста приложения");

            InitInstallers();
        }

        /// <summary>
        /// Остановка
        /// </summary>
        public void Stop()
        {
            Log.Debug("Остановка контекста приложения");
        }

        /// <summary>
        /// Сопоставляет виртуальный путь с физическим путем на сервере.
        /// </summary>
        /// <param name="virtualPath">
        /// Виртуальный путь.
        /// </param>
        /// <returns></returns>
        public abstract string MapPath(string virtualPath);

        public string CurUserId()
        {
            return System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.User != null
                ? System.Web.HttpContext.Current.User.Identity.GetUserId()
                : null;
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetUrl();

        public abstract string ApplicationPhysicalPath();

        /// <summary>
        /// Запуск установщиков модулей
        /// </summary>
        private void InitInstallers()
        {
            var directory = MapPath("bin");

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName().FullName).ToHashSet();

            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                var asmName = AssemblyName.GetAssemblyName(file);

                if (!loadedAssemblies.Contains(asmName.FullName))
                {
                    Assembly.LoadFile(file);
                    loadedAssemblies.Add(asmName.FullName);
                }
            }

            var installers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.Is<IInstaller>())
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface)
                .Select(x => (IInstaller) Activator.CreateInstance(x))
                .ToArray();

            foreach (var installer in installers)
            {
                installer.Install();
                Log.Debug(installer.Name);
            }

            var afterInstallInterceptors = Container.GetAllInstances<IAfterInstallInterceptor>();

            foreach (var afterInstallInterceptor in afterInstallInterceptors)
            {
                afterInstallInterceptor.Run();
            }
        }
    }
}