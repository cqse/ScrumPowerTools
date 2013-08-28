using System;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;


namespace ScrumPowerTools.Model
{
    public class ShowChangesetsModel
    {
        private readonly WorkItemCollector workItemCollector;
        private readonly WorkItemStore workItemStore;
        private readonly VersionControlServer versionControlServer;
        private readonly IVisualStudioAdapter visualStudioAdapter;
        private readonly WorkItemSelectionService workItemSelectionService;
        
        internal event EventHandler<ShowChangesetsEventArgs> ShowChangesets = delegate { };

        public ShowChangesetsModel(WorkItemSelectionService workItemSelectionService, WorkItemCollector workItemCollector, 
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
            var collectorStrategy = new ChangesetCollectorStrategy(workItemStore, versionControlServer, visualStudioAdapter);

            workItemCollector.CollectItems(workItemSelectionService.GetFirstSelected(), collectorStrategy);

            ShowChangesets(this, new ShowChangesetsEventArgs(collectorStrategy.Changesets));
        }
    }
}