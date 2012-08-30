using EnvDTE;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.Services;
using ScrumPowerTools.TfsIntegration;
using ScrumPowerTools.ViewModels;
using ScrumPowerTools.Views;
using System.Linq;

namespace ScrumPowerTools.Controllers
{
    public class MenuCommandController
    {
        private readonly DTE dte;
        private readonly DocumentService docService;
        private readonly ITeamProjectCollectionProvider teamProjectCollectionProvider;
        private readonly GeneralOptions options;
        private readonly TfsQueryShortcutAssigner tfsQueryShortcutAssigner;
        private readonly TfsQueryShortcutOpener tfsQueryShortcutOpener;
        private readonly uint[] assignShortcutCommandIdentifiers;
        private uint[] openTfsQueryCommandIdentifiers;

        public MenuCommandController(DTE dte, DocumentService docService,
                                     ITeamProjectCollectionProvider teamProjectCollectionProvider,
                                     GeneralOptions options, TfsQueryShortcutAssigner tfsQueryShortcutAssigner,
                                     TfsQueryShortcutOpener tfsQueryShortcutOpener)
        {
            this.dte = dte;
            this.docService = docService;
            this.teamProjectCollectionProvider = teamProjectCollectionProvider;
            this.options = options;
            this.tfsQueryShortcutAssigner = tfsQueryShortcutAssigner;
            this.tfsQueryShortcutOpener = tfsQueryShortcutOpener;

            assignShortcutCommandIdentifiers = new[]
            {
                MenuCommands.AssignTfsQueryShortcut1, MenuCommands.AssignTfsQueryShortcut2,
                MenuCommands.AssignTfsQueryShortcut3, MenuCommands.AssignTfsQueryShortcut4,
                MenuCommands.AssignTfsQueryShortcut5
            };

            openTfsQueryCommandIdentifiers = new[]
            {
                MenuCommands.OpenTfsQuery1, MenuCommands.OpenTfsQuery2,
                MenuCommands.OpenTfsQuery3, MenuCommands.OpenTfsQuery4,
                MenuCommands.OpenTfsQuery5
            };
        }

        public bool Execute(uint commandId)
        {
            if (commandId == MenuCommands.ShowAffectedChangesetFiles)
            {
                var workItemSelectionService = new WorkItemSelectionService(dte, docService);
                TfsTeamProjectCollection tpc = teamProjectCollectionProvider.GetCurrent();
                var workItemStore = tpc.GetService<WorkItemStore>();
                var versionControlServer = tpc.GetService<VersionControlServer>();
                var workItemCollector = new WorkItemCollector(workItemStore, versionControlServer);
                var model = new ShowChangesetItemsModel(workItemSelectionService, workItemCollector);
                var view = new ShowChangesetItemsView(dte);

                view.ConnectTo(model);

                model.Execute();

                return true;
            }

            if (commandId == MenuCommands.ShowChangesetsWithAffectedFiles)
            {
                var workItemSelectionService = new WorkItemSelectionService(dte, docService);
                TfsTeamProjectCollection tpc = teamProjectCollectionProvider.GetCurrent();
                var workItemStore = tpc.GetService<WorkItemStore>();
                var versionControlServer = tpc.GetService<VersionControlServer>();
                var workItemCollector = new WorkItemCollector(workItemStore, versionControlServer);
                var model = new ShowChangesetsModel(workItemSelectionService, workItemCollector);
                var view = new ShowChangesetsView(dte);

                view.ConnectTo(model);

                model.Execute();

                return true;
            }

            if (commandId == MenuCommands.ShowReviewWindow)
            {
                var workItemSelectionService = IoC.GetInstance<WorkItemSelectionService>();

                if (workItemSelectionService.HasSelection())
                {
                    IoC.GetInstance<EventAggregator>().Publish(new ShowReviewWindowMessage()
                    {
                        WorkItemId = workItemSelectionService.GetFirstSelected()
                    });
                }

                return true;
            }

            if (openTfsQueryCommandIdentifiers.Contains(commandId))
            {
                tfsQueryShortcutOpener.Open(commandId);
                return true;
            }
            if (assignShortcutCommandIdentifiers.Contains(commandId))
            {
                tfsQueryShortcutAssigner.Assign(commandId);
                return true;
            }

            return false;
        }

        public bool CanExecute(uint commandId)
        {
            if (options.IsEnabled(commandId))
            {
                return new WorkItemSelectionService(dte, docService).HasSelection();
            }

            return false;
        }
    }
}