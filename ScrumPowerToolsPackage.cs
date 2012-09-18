using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using Microsoft.VisualStudio.Shell;
using ScrumPowerTools.Controllers;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.Services;
using ScrumPowerTools.TfsIntegration;


namespace ScrumPowerTools
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "0.1", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ReviewToolWindow))]
    [ProvideOptionPage(typeof(GeneralOptions),"Scrum Power Toolss", "General", 110, 1001, false)]
    [Guid(Identifiers.PackageId)]
    [ProvideAutoLoad("{e13eedef-b531-4afe-9725-28a69fa4f896}")] //Auto load when having connection with TFS
    public sealed class ScrumPowerToolsPackage : Package, IToolWindowActivator, IPackageServiceProvider
    {
        public ScrumPowerToolsPackage()
        {
            IoC.Setup(Assembly.GetExecutingAssembly());
            IoC.Register<IToolWindowActivator>(this);
            IoC.Register<IPackageServiceProvider>(this);
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

#if VS11
            IoC.Register<ITeamProjectCollectionProvider>(new Vs11TeamProjectCollectionProvider());
#else
            IoC.Register<ITeamProjectCollectionProvider>(new Vs10TeamProjectCollectionProvider());
#endif

            var projectUriProvider = IoC.GetInstance<ITeamProjectCollectionProvider>();

            var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
            var documentService = (DocumentService)GetGlobalService(typeof(DocumentService));

            IoC.Register(new WorkItemSelectionService(dte, documentService));
            IoC.Register(new ShellDocumentOpener(this));
            IoC.Register(new TfsUiServices(dte));

            new QueryResultsTotalizerController(documentService, dte.StatusBar, projectUriProvider);

            var options = (GeneralOptions)GetDialogPage(typeof(GeneralOptions));
            var tfsQueryShortcutStore = new TfsQueryShortcutStore(options);
            var tfsQueryShortcutAssigner = new TfsQueryShortcutAssigner(tfsQueryShortcutStore);
            var tfsQueryShortcutOpener = new TfsQueryShortcutOpener(documentService, projectUriProvider, tfsQueryShortcutStore);

            IoC.Register(options);
            IoC.Register(tfsQueryShortcutAssigner);
            IoC.Register(tfsQueryShortcutOpener);

            MenuCommandHandlerBinder.Bind(GetService<IMenuCommandService>());
            ComboBoxCommandHandlerBinder.Bind(GetService<IMenuCommandService>());
        }

        public void Activate<T>()
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            const bool createWhenNotFound = true;
            ToolWindowPane window = FindToolWindow(typeof(T), 0, createWhenNotFound);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Resources.CanNotCreateWindow");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public T GetService<T>() where T : class
        {
            return (T)GetService(typeof(T));
        }
    }
}