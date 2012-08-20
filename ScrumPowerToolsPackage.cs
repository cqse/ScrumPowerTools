using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
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
using ScrumPowerTools.ViewModels;


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
    public sealed class ScrumPowerToolsPackage : Package, IOleCommandTarget, IToolWindowActivator, IPackageServiceProvider
    {
        private MenuCommandController menuCommandController;
        private const int OLECMDERR_E_UNKNOWNGROUP = unchecked((int)0x80040104);

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
            var tfsQueryShortcutOpener = new TfsQueryShortcutOpener(documentService, projectUriProvider, options);

            menuCommandController = new MenuCommandController(dte, documentService, projectUriProvider, options, tfsQueryShortcutOpener);
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint commandId, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup != Identifiers.CommandGroupId)
            {
                return OLECMDERR_E_UNKNOWNGROUP;
            }

            
            uint cmdId = prgCmds[0].cmdID;

            switch (cmdId)
            {
                case MenuCommands.ShowReviewWindow:
                case MenuCommands.ShowAffectedChangesetFiles:
                case MenuCommands.ShowChangesetsWithAffectedFiles:

                    if (menuCommandController.CanExecute(cmdId))
                    {
                        prgCmds[0].cmdf = (int)OLECMDF.OLECMDF_ENABLED | (int)OLECMDF.OLECMDF_SUPPORTED;
                    }
                    else
                    {
                        prgCmds[0].cmdf = (int)OLECMDF.OLECMDF_INVISIBLE | (int)OLECMDF.OLECMDF_SUPPORTED;                            
                    }
                    break;

                case MenuCommands.OpenTfsQuery1:
                case MenuCommands.ChangeReviewGrouping:

                    prgCmds[0].cmdf = (int)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);

                    break;

                default:
                    return OLECMDERR_E_UNKNOWNGROUP;
            }

            return VSConstants.S_OK;
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup != Identifiers.CommandGroupId)
            {
                return OLECMDERR_E_UNKNOWNGROUP;
            }

            var eventAggregator = IoC.GetInstance<EventAggregator>();

            if (menuCommandController.Execute(commandId))
            {
                return VSConstants.S_OK;
            }

            if (commandId == MenuCommands.FillReviewGroupingComboList)
            {
                if (pvaOut == IntPtr.Zero)
                {
                    throw (new ArgumentException("Resources.InParamIllegal"));
                }

                var @event = new RequestReviewGroupingChoicesEvent();
                eventAggregator.Publish(@event);

                Marshal.GetNativeVariantForObject(@event.Choices, pvaOut);

                return VSConstants.S_OK;
            }

            if (commandId == MenuCommands.ChangeReviewGrouping)
            {
                if (pvaOut != IntPtr.Zero)
                {
                    var @event = new RequestSelectedReviewGroupingEvent();
                    eventAggregator.Publish(@event);

                    Marshal.GetNativeVariantForObject(@event.Selection, pvaOut);
                }
                else if (pvaIn != IntPtr.Zero)
                {
                    var selectedGrouping = Marshal.GetObjectForNativeVariant(pvaIn) as string;

                    eventAggregator.Publish(new ReviewGroupingSelectedEvent()
                    {
                        Selection = selectedGrouping
                    });
                }

                return VSConstants.S_OK;
            }

            if (commandId == MenuCommands.CollapseAllReviewItems)
            {
                eventAggregator.Publish(new CollapseAllReviewItemsEvent());
                return VSConstants.S_OK;
            }

            if (commandId == MenuCommands.ExpandAllReviewItems)
            {
                eventAggregator.Publish(new ExpandAllReviewItemsEvent());
                return VSConstants.S_OK;
            }


            return OLECMDERR_E_UNKNOWNGROUP;
        }

        public void Activate<T>()
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            bool createWhenNotFound = true;
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