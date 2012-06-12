using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace XamlStyler.VSPackage
{
    /// <summary>
    /// Provides a utility function on top of the <see cref="Package"/> for creating fully functional VSPackages.
    /// </summary>
    public abstract class VSPackageBase : Package
    {
        /// <summary>
        /// Gets the top-level object in the Visual Studio automation object model. 
        /// </summary>
        protected DTE DTE
        {
            get
            {
                return this.GetService<DTE>();
            }
        }

        /// <summary>
        /// Gets a <see cref="IVsUIShell"/> object, providing access to basic windowing functionality,
        /// </summary>
        protected IVsUIShell VsUIShell
        {
            get
            {
                return this.GetService<IVsUIShell>();
            }
        }

        /// <summary>
        /// Gets a <see cref="Events"/> object, providing access to all events in the extensibility model.
        /// </summary>
        protected Events Events
        {
            get
            {
                return DTE.Events as Events;
            }
        }

        /// <summary>
        /// Gets a type-based service from the VSPackage service container.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve.</typeparam>
        /// <returns>An instance of the requested service, or <c>null</c> if the service could not be found.</returns>
        protected TService GetService<TService>()
            where TService : class
        {
            return this.GetService(typeof(TService)) as TService;
        }

        /// <summary>
        /// Gets a type-based service from the VSPackage service container, as a given implementation.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve.</typeparam>
        /// <typeparam name="TService">The type of specific implementation of the retrieved service.</typeparam>
        /// <returns>An instance of the requested service, or <c>null</c> if the service could not be found.</returns>
        protected TImplementation GetService<TService, TImplementation>()
            where TService : class
            where TImplementation : class
        {
            return this.GetService(typeof(TService)) as TImplementation;
        }
    }
}
