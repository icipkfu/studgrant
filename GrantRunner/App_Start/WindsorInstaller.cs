namespace GrantRunner
{
    using System;
    using System.Web.Http;
    using System.Web.Mvc;
    using Castle.Core.Internal;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Core.Ioc;

    public static class WindsorInstaller
    {
        public static void RegisterComponents(IWindsorContainer container)
        {
            var assemblyProvider =
                new AssemblyFilter(AppDomain.CurrentDomain.RelativeSearchPath).FilterByName(
                    assemblyName => assemblyName.Name.StartsWith("Grant"));

            InstallComponents(assemblyProvider, container);

            var controllerFactory = new WindsorControllerFactory(container.Kernel, new DefaultControllerFactory());

            container.Register(Component.For<IControllerFactory>().UsingFactoryMethod(x => controllerFactory).LifeStyle.Singleton);

            //ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        private static void InstallComponents(IAssemblyProvider assemblyProvider, IWindsorContainer container)
        {
            var assemblies = assemblyProvider.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                container.Install(FromAssembly.Instance(assembly));

                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract)
                    {
                        if (type.IsSubclassOf(typeof(Controller)))
                        {
                            container.RegisterController(type);
                        }
                        if (type.IsSubclassOf(typeof(ApiController)))
                        {
                            container.RegisterApiController(type);
                        }

                    }
                }
            }
        }
    }
}