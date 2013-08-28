using System;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    public class ShowChangesetItemsModel
    {
        private readonly WorkItemCollector workItemCollector;
        private readonly WorkItemStore workItemStore;
        private readonly VersionControlServer versionControlServer;
        private readonly IVisualStudioAdapter visualStudioAdapter;
        private readonly WorkItemSelectionService workItemSelectionService;
        public event EventHandler<ShowChangesetItemsEventArgs> ShowChangesetItems = delegate {};

        public ShowChangesetItemsModel(WorkItemSelectionService workItemSelectionService, WorkItemCollector workItemCollector,
            WorkItemStore workItemStore, VersionControlServer versionControlServer, IVisualStudioAdapter visualStudioAdapter)
        {
            this.workItemSelectionService = workItemSelectionService;
            this.workItemCollector = workItemCollector;
            this.workItemStore = workItemStore;
            this.versionControlServer = versionControlServer;
            this.visualStudioAdapter = visualStudioAdapter;
        }

        public void Execute()
        {
            var collector = new ChangesetServerItemCollectorStrategy(workItemStore, versionControlServer, visualStudioAdapter);

            workItemCollector.CollectItems(workItemSelectionService.GetFirstSelected(), collector);

            ShowChangesetItems(this, new ShowChangesetItemsEventArgs(collector.ServerItems));
        }
    }
}