using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    internal class ChangesetVisitor
    {
        public event EventHandler<ChangesetVisitEventArgs> ChangesetVisit = delegate { };

        private readonly VersionControlServer versionControlServer;
        private readonly IVisualStudioAdapter visualStudioAdapter;
        private readonly WorkItemVisitor workItemVisitor;
        private readonly Workspace workspace;

        public ChangesetVisitor(WorkItemStore store, VersionControlServer versionControlServer, IVisualStudioAdapter visualStudioAdapter)
        {
            this.versionControlServer = versionControlServer;
            this.visualStudioAdapter = visualStudioAdapter;

            workItemVisitor = new WorkItemVisitor(store);
            workItemVisitor.WorkItemVisit += OnWorkItemVisit;

            workspace = visualStudioAdapter.GetCurrentWorkSpace();
        }

        void OnWorkItemVisit(object sender, WorkItemVisitEventArgs e)
        {
            VisitChangesets(e.WorkItem);
        }

        private void VisitChangesets(WorkItem workItem)
        {
            var externalLinks = workItem.Links.OfType<ExternalLink>();

            foreach (ExternalLink externalLink in externalLinks)
            {
                VisitChangeset(externalLink);
            }
        }

        private void VisitChangeset(ExternalLink externalLink)
        {
            var artifactUri = new Uri(externalLink.LinkedArtifactUri);

            VersionControlArtifactType artifactType = versionControlServer.ArtifactProvider.GetArtifactType(artifactUri);

            if (artifactType == VersionControlArtifactType.Changeset)
            {
                var changeSet = versionControlServer.ArtifactProvider.GetChangeset(artifactUri);

                ChangesetVisit(this, new ChangesetVisitEventArgs(changeSet, ConvertToLocal(changeSet.Changes), workspace));
            }
        }

        private IEnumerable<string> ConvertToLocal(Change[] changes)
        {
            if (workspace == null)
            {
                return new string[0];
            }

            string[] serverItems = changes.Select(c => c.Item.ServerItem).ToArray();
            var localizedServerItems = new List<string>();

            foreach (string serverItem in serverItems)
            {
                string displayItem = null;

                if (workspace != null)
                {
                    displayItem = workspace.TryGetLocalItemForServerItem(serverItem);
                }

                if (string.IsNullOrEmpty(displayItem))
                {
                    displayItem = serverItem;
                }

                localizedServerItems.Add(displayItem);
            }

            return localizedServerItems;
        }

        public void Visit(WorkItem workItem)
        {
            workItemVisitor.Visit(workItem);
        }
    }
}