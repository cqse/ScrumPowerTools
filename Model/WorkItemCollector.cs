using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    public class WorkItemCollector
    {
        private readonly WorkItemStore workItemStore;
        private readonly VersionControlServer versionControlServer;
        private readonly IVisualStudioAdapter visualStudioAdapter;

        public WorkItemCollector(WorkItemStore workItemStore, VersionControlServer versionControlServer, IVisualStudioAdapter visualStudioAdapter)
        {
            this.workItemStore = workItemStore;
            this.versionControlServer = versionControlServer;
            this.visualStudioAdapter = visualStudioAdapter;
        }

        internal void CollectItems(int workItemId, IWorkItemCollectorStrategy strategy)
        {
            CollectForWorkItem(workItemId, strategy, visualStudioAdapter);
        }

        private void CollectForWorkItem(int selectedItemIds, IWorkItemCollectorStrategy strategy, IVisualStudioAdapter visualStudioAdapter)
        {
            WorkItem workItem = workItemStore.GetWorkItem(selectedItemIds);

            strategy.Collect(workItem, workItemStore, versionControlServer, visualStudioAdapter);
        }
    }
}