using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Mvvm.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Prism.Windows.Store.Unity
{
    public abstract class UnityMvvmAppBase : MvvmAppBase
    {
        /// <summary>
        /// Gets the default <see cref="IUnityContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IUnityContainer"/> instance.</value>
        public IUnityContainer Container { get; protected set; }

        /// <summary>
        /// Creates the <see cref="IUnityContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IUnityContainer"/>.</returns>
        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected override object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container = CreateContainer();
            if (Container == null)
            {
                throw new InvalidOperationException("NullUnityContainerException");
            }

            ConfigureContainer();
            ConfigureServiceLocator();

            return Task.FromResult<object>(null);
        }


        /// <summary>
        /// Configures the <see cref="IUnityContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Container.AddNewExtension<UnityMvvmAppBaseExtension>();

            Container.RegisterInstance<INavigationService>(NavigationService);

            RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);

        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => Container.Resolve<IServiceLocator>());
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="fromType">The interface type to register.</param>
        /// <param name="toType">The type implementing the interface.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing(Type fromType, Type toType, bool registerAsSingleton)
        {
            if (fromType == null)
            {
                throw new ArgumentNullException("fromType");
            }
            if (toType == null)
            {
                throw new ArgumentNullException("toType");
            }
            if (!Container.IsTypeRegistered(fromType))
            {
                if (registerAsSingleton)
                {
                    Container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
                }
                else
                {
                    Container.RegisterType(fromType, toType);
                }
            }
        }
    }
}
