using System;

namespace ScrumPowerTools.Model
{
    public class ShowChangesetItemsModel
    {
        private readonly WorkItemCollector workItemCollector;
        private readonly WorkItemSelectionService workItemSelectionService;
        public event EventHandler<ShowChangesetItemsEventArgs> ShowChangesetItems = delegate {};

        public ShowChangesetItemsModel(WorkItemSelectionService workItemSelectionService, WorkItemCollector workItemCollector)
        {
            this.workItemSelectionService = workItemSelectionService;
            this.workItemCollector = workItemCollector;
        }

        public void Execute()
        {
            var collector = new ChangesetServerItemCollectorStrategy();

            workItemCollector.CollectItems(workItemSelectionService.GetFirstSelected(), collector);

            ShowChangesetItems(this, new ShowChangesetItemsEventArgs(collector.ServerItems));
        }
    }
}