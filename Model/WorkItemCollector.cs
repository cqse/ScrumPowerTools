using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ScrumPowerTools.Model
{
    public class WorkItemCollector
    {
        private readonly WorkItemStore workItemStore;

        public WorkItemCollector(WorkItemStore workItemStore)
        {
            this.workItemStore = workItemStore;
        }

        internal void CollectItems(int workItemId, IWorkItemCollectorStrategy strategy)
        {
            CollectForWorkItem(workItemId, strategy);
        }

        private void CollectForWorkItem(int selectedItemIds, IWorkItemCollectorStrategy strategy)
        {
            WorkItem workItem = workItemStore.GetWorkItem(selectedItemIds);

            strategy.Collect(workItem);
        }
    }
}