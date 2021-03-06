using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.TfsIntegration;
using ScrumPowerTools.Extensibility.Service;

namespace ScrumPowerTools.Model.Review
{
    public class ReviewModel
    {
        private readonly VersionControlServer versionControlServer;
        private readonly WorkItemStore workItemStore;
        private readonly IVisualStudioAdapter teamProjectCollectionProvider;

        public ReviewModel()
        {
            teamProjectCollectionProvider = IoC.GetInstance<IVisualStudioAdapter>();
            var tpc = teamProjectCollectionProvider.GetCurrent();

            workItemStore = tpc.GetService<WorkItemStore>();
            versionControlServer = tpc.GetService<VersionControlServer>();
        }

        public string Title { get; set; }

        public IEnumerable<ReviewItemModel> ItemsToReview { get; private set; }

        public void Review(int workItemId, IReviewItemFilter filter)
        {
            if (workItemId <= 0)
            {
                ItemsToReview = new ReviewItemModel[0];
            }

            var workItemCollector = new WorkItemCollector(workItemStore);
            var collectorStrategy = new ReviewItemCollectorStrategy(workItemStore, versionControlServer, teamProjectCollectionProvider, filter);

            workItemCollector.CollectItems(workItemId, collectorStrategy);

            ItemsToReview = collectorStrategy.Items;

            var workItem = workItemStore.GetWorkItem(workItemId);

            Title = string.Format("Review - {0} {1} - {2}", workItem.Type.Name, workItem.Id, workItem.Title);
        }

        public void CompareInitialVersionWithLatestChange(string serverItem)
        {
            int firstChangesetId = ItemsToReview
                .Where(reviewItem => reviewItem.ServerItem == serverItem)
                .Min(reviewItem => reviewItem.ChangesetId);

            int lastChangesetId = ItemsToReview
                .Where(reviewItem => reviewItem.ServerItem == serverItem)
                .Max(reviewItem => reviewItem.ChangesetId);

            var diffirentiator = new TfsItemDifferentiator();
            diffirentiator.CompareInitialVersionWithLatestChange(serverItem, firstChangesetId, lastChangesetId);
        }

        public void CompareWithPreviousVersion(string serverItem, int changesetId)
        {
            var differentiator = new TfsItemDifferentiator();
            differentiator.CompareWithPreviousVersion(serverItem, changesetId);
        }

        public void ShowChangesetDetails(int changesetId)
        {
            var visualStudioAdapter = IoC.GetInstance<IVisualStudioAdapter>();
            visualStudioAdapter.ShowChangesetDetails(changesetId);
        }
    }
}