using System;


namespace ScrumPowerTools.Model
{
    public class ShowChangesetsModel
    {
        private readonly WorkItemCollector workItemCollector;
        private readonly WorkItemSelectionService workItemSelectionService;
        
        internal event EventHandler<ShowChangesetsEventArgs> ShowChangesets = delegate { };

        public ShowChangesetsModel(WorkItemSelectionService workItemSelectionService, WorkItemCollector workItemCollector)
        {
            this.workItemSelectionService = workItemSelectionService;
            this.workItemCollector = workItemCollector;
        }

        public void Execute()
        {
            var collectorStrategy = new ChangesetCollectorStrategy();

            workItemCollector.CollectItems(workItemSelectionService.GetFirstSelected(), collectorStrategy);

            ShowChangesets(this, new ShowChangesetsEventArgs(collectorStrategy.Changesets));
        }
    }
}