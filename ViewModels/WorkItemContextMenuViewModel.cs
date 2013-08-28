using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.TfsIntegration;
using ScrumPowerTools.Views;

namespace ScrumPowerTools.ViewModels
{
    [Export(typeof(IMenuCommandHandler))]
    [HandlesMenuCommand(MenuCommands.ShowAffectedChangesetFiles)]
    [HandlesMenuCommand(MenuCommands.ShowChangesetsWithAffectedFiles)]
    [HandlesMenuCommand(MenuCommands.ShowReviewWindow)]
    public class WorkItemContextMenuViewModel : IMenuCommandHandler
    {
        private readonly GeneralOptions options;
        private readonly WorkItemSelectionService workItemSelectionService;
        private readonly IVisualStudioAdapter visualStudioAdapter;

        private readonly Dictionary<int, Action> commandHandlerMappings;
        private readonly Dictionary<int, Feature> commandFeatureMappings;

        [ImportingConstructor]
        public WorkItemContextMenuViewModel(GeneralOptions options, WorkItemSelectionService workItemSelectionService,
                                            IVisualStudioAdapter visualStudioAdapter)
        {
            this.options = options;
            this.workItemSelectionService = workItemSelectionService;
            this.visualStudioAdapter = visualStudioAdapter;

            commandHandlerMappings = new Dictionary<int, Action>
            {
                {MenuCommands.ShowAffectedChangesetFiles, ShowAffectedChangesetFiles},
                {MenuCommands.ShowChangesetsWithAffectedFiles, ShowChangesetsWithAffectedFiles},
                {MenuCommands.ShowReviewWindow, ShowReviewWindow}
            };

            commandFeatureMappings = new Dictionary<int, Feature>
            {
                {MenuCommands.ShowAffectedChangesetFiles, Feature.ShowAffectedChangesetFiles},
                {MenuCommands.ShowChangesetsWithAffectedFiles, Feature.ShowChangesetsWithAffectedFiles},
                {MenuCommands.ShowReviewWindow, Feature.Review}
            };
        }

        private void ShowChangesetsWithAffectedFiles()
        {
            TfsTeamProjectCollection tpc = visualStudioAdapter.GetCurrent();
            var workItemStore = tpc.GetService<WorkItemStore>();
            var versionControlServer = tpc.GetService<VersionControlServer>();
            var workItemCollector = new WorkItemCollector(workItemStore);
            var model = new ShowChangesetsModel(workItemSelectionService, workItemCollector, workItemStore, versionControlServer, visualStudioAdapter);

            var dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
            var view = new ShowChangesetsView(dte);

            view.ConnectTo(model);

            model.Execute();
        }

        private void ShowAffectedChangesetFiles()
        {
            TfsTeamProjectCollection tpc = visualStudioAdapter.GetCurrent();
            var workItemStore = tpc.GetService<WorkItemStore>();
            var versionControlServer = tpc.GetService<VersionControlServer>();
            var workItemCollector = new WorkItemCollector(workItemStore);
            var model = new ShowChangesetItemsModel(workItemSelectionService, workItemCollector, workItemStore, versionControlServer, visualStudioAdapter);

            var dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
            var view = new ShowChangesetItemsView(dte);

            view.ConnectTo(model);

            model.Execute();
        }

        private void ShowReviewWindow()
        {
            if (workItemSelectionService.HasSelection())
            {
                IoC.GetInstance<EventAggregator>().Publish(new ShowReviewWindowMessage()
                {
                    WorkItemId = workItemSelectionService.GetFirstSelected()
                });
            }
        }

        public void Execute(int commandId)
        {
            commandHandlerMappings[commandId]();
        }

        public bool CanExecute(int commandId)
        {
            return options.IsEnabled(commandFeatureMappings[commandId]) && workItemSelectionService.HasSelection();
        }
    }
}