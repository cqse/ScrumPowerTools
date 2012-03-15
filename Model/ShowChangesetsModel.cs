using System;
using EnvDTE;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using ScrumPowerTools.Models;

namespace ScrumPowerTools.Model
{
    public class ShowChangesetsModel
    {
        private readonly WorkItemCollector workItemCollector;
        private readonly WorkItemSelectionService workItemSelectionService;
        internal event EventHandler<ShowChangesetsEventArgs> ShowChangesets = delegate { };

        public ShowChangesetsModel(DTE dte, DocumentService docService, IVsTeamExplorer teamExplorer)
        {
            workItemSelectionService = new WorkItemSelectionService(dte, docService);
            workItemCollector = new WorkItemCollector(teamExplorer);
        }

        public void Execute()
        {
            var collectorStrategy = new ChangesetCollectorStrategy();

            workItemCollector.CollectItems(workItemSelectionService.GetFirstSelected(), collectorStrategy);

            ShowChangesets(this, new ShowChangesetsEventArgs(collectorStrategy.Changesets));
        }
    }
}