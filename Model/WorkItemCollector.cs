using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    internal class WorkItemCollector
    {
        private readonly WorkItemStore workItemStore;
        private readonly VersionControlServer versionControlServer;

        public WorkItemCollector(ITeamProjectCollectionProvider teamExplorer)
        {
            var tpc = teamExplorer.GetCurrent();

            workItemStore = tpc.GetService<WorkItemStore>();
            versionControlServer = tpc.GetService<VersionControlServer>();
        }

        public WorkItemCollector(WorkItemStore workItemStore, VersionControlServer versionControlServer)
        {
            this.workItemStore = workItemStore;
            this.versionControlServer = versionControlServer;
        }

        internal void CollectItems(int workItemId, IWorkItemCollectorStrategy strategy)
        {
            CollectForWorkItem(workItemId, strategy);
        }

        private void CollectForWorkItem(int selectedItemIds, IWorkItemCollectorStrategy strategy)
        {

            WorkItem workItem = workItemStore.GetWorkItem(selectedItemIds);

            strategy.Collect(workItem, workItemStore, versionControlServer);
        }
    }
}