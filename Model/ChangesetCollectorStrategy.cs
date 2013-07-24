using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    internal class ChangesetModel
    {
        public string Title { get; internal set; }

        public int Number { get; internal set; }

        public IEnumerable<string> ServerItems { get; internal set; }

        public DateTime CreationDate { get; set; }

        public string Committer { get; internal set; }
    }

    internal class ChangesetCollectorStrategy : IWorkItemCollectorStrategy
    {
        private IList<ChangesetModel> changesets;
        public IEnumerable<ChangesetModel> Changesets
        {
            get { return changesets; }
        }

        void OnChangesetVisit(object sender, ChangesetVisitEventArgs e)
        {
            var changesetModel = new ChangesetModel();

            changesetModel.Number = e.Changeset.ChangesetId;
            changesetModel.Title = e.Changeset.Comment;
            changesetModel.CreationDate = e.Changeset.CreationDate;
            changesetModel.Committer = e.Committer;

            var serverItems = new List<string>();

            foreach (string serverItem in e.LocalizedServerItems)
            {
                serverItems.Add(serverItem);
            }

            changesetModel.ServerItems = serverItems;

            changesets.Add(changesetModel);
        }

        public void Collect(WorkItem workItem, WorkItemStore store, VersionControlServer versionControlServer, IVisualStudioAdapter visualStudioAdapter)
        {
            var changesetVisitor = new ChangesetVisitor(store, versionControlServer, visualStudioAdapter);
            changesetVisitor.ChangesetVisit += OnChangesetVisit;

            changesets = new List<ChangesetModel>();

            changesetVisitor.Visit(workItem);
        }
    }
}