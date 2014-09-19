using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;
using ScrumPowerTools.Extensibility.Service;

namespace ScrumPowerTools.Model.Review
{
    internal class ReviewItemCollectorStrategy : IWorkItemCollectorStrategy
    {
        private readonly WorkItemStore store;
        private readonly VersionControlServer versionControlServer;
        private readonly IVisualStudioAdapter visualStudioAdapter;
        private readonly IReviewItemFilter filter;
        private List<ReviewItemModel> items;

        public IEnumerable<ReviewItemModel> Items
        {
            get { return items; }
        }

        public ReviewItemCollectorStrategy(WorkItemStore store, VersionControlServer versionControlServer, IVisualStudioAdapter visualStudioAdapter, IReviewItemFilter filter)
        {
            this.store = store;
            this.versionControlServer = versionControlServer;
            this.visualStudioAdapter = visualStudioAdapter;
            this.filter = filter;
        }

        public void Collect(WorkItem workItem)
        {
            var changesetVisitor = new ChangesetVisitor(store, versionControlServer, visualStudioAdapter);
            changesetVisitor.ChangesetVisit += OnChangesetVisit;

            items = new List<ReviewItemModel>();

            changesetVisitor.Visit(workItem);
        }

        void OnChangesetVisit(object sender, ChangesetVisitEventArgs e)
        {
            foreach (Change change in e.Changeset.Changes)
            {
                var reviewItemModel = new ReviewItemModel(e);

                reviewItemModel.LocalFilePath = e.Workspace.TryGetLocalItemForServerItem(change.Item.ServerItem);
                reviewItemModel.ServerItem = change.Item.ServerItem;
                reviewItemModel.Change = (change.ChangeType & (~ChangeType.Encoding)).ToString();

                if (this.filter == null || this.filter.IsIncluded(reviewItemModel))
                {
                    items.Add(reviewItemModel);
                }
            }
        }
    }
}