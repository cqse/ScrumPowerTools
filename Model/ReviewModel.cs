using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.Interfaces;

namespace ScrumPowerTools.Model
{
    public class ReviewModel
    {
        private readonly VersionControlServer versionControlServer;
        private readonly WorkItemStore workItemStore;

        public ReviewModel()
        {
            var teamExplorer = IoC.GetInstance<IVsTeamExplorer>();
            var tpc = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(teamExplorer.GetProjectContext().DomainUri));

            workItemStore = tpc.GetService<WorkItemStore>();
            versionControlServer = tpc.GetService<VersionControlServer>();
        }

        public string Title { get; set; }
        public IEnumerable<ReviewItemModel> ItemsToReview { get; private set; }

        public void Review(int workItemId)
        {
            if (workItemId <= 0)
            {
                ItemsToReview = new ReviewItemModel[0];
            }

            var workItemCollector = new WorkItemCollector(workItemStore, versionControlServer);
            var collectorStrategy = new ReviewItemCollectorStrategy();

            workItemCollector.CollectItems(workItemId, collectorStrategy);

            ItemsToReview = collectorStrategy.Items;

            var workItem = workItemStore.GetWorkItem(workItemId);

            Title = string.Format("Review - {0} - {1}", workItem.Type.Name, workItem.Title);
        }
    }
}