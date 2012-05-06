using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ScrumPowerTools.Model
{
    internal class ReviewItemCollectorStrategy : IWorkItemCollectorStrategy
    {
        private List<ReviewItemModel> items;
        public IEnumerable<ReviewItemModel> Items
        {
            get { return items; }
        }

        void OnChangesetVisit(object sender, ChangesetVisitEventArgs e)
        {
            foreach (Change change in e.Changeset.Changes)
            {
                var reviewItemModel = new ReviewItemModel(e);

                reviewItemModel.LocalFilePath = e.Workspace.TryGetLocalItemForServerItem(change.Item.ServerItem);
                reviewItemModel.ServerItem = change.Item.ServerItem;
                reviewItemModel.Change = change.ChangeType.ToString();

                items.Add(reviewItemModel);
            }
        }

        public void Collect(WorkItem workItem, WorkItemStore store, VersionControlServer versionControlServer)
        {
            var changesetVisitor = new ChangesetVisitor(store, versionControlServer);
            changesetVisitor.ChangesetVisit += OnChangesetVisit;

            items = new List<ReviewItemModel>();

            changesetVisitor.Visit(workItem);
        }
    }
}