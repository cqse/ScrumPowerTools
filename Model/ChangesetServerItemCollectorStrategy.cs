using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    internal interface IWorkItemCollectorStrategy
    {
        void Collect(WorkItem workItem);
    }

    internal class ChangesetServerItemCollectorStrategy : IWorkItemCollectorStrategy
    {
        private readonly WorkItemStore store;
        private readonly VersionControlServer versionControlServer;
        private readonly IVisualStudioAdapter visualStudioAdapter;
        public IEnumerable<string> ServerItems { get; private set; }

        private IList<string> serverItems;

        public ChangesetServerItemCollectorStrategy( WorkItemStore store, VersionControlServer versionControlServer, IVisualStudioAdapter visualStudioAdapter)
        {
            this.store = store;
            this.versionControlServer = versionControlServer;
            this.visualStudioAdapter = visualStudioAdapter;
        }

        public void Collect(WorkItem workItem)
        {
            var changesetVisitor = new ChangesetVisitor(store, versionControlServer, visualStudioAdapter);
            changesetVisitor.ChangesetVisit += OnChangesetVisit;

            serverItems = new List<string>();

            changesetVisitor.Visit(workItem);

            ServerItems = serverItems;
        }

        void OnChangesetVisit(object sender, ChangesetVisitEventArgs e)
        {
            foreach (string serverItem in e.LocalizedServerItems)
            {
                if (!serverItems.Any(si => si == serverItem))
                {
                    serverItems.Add(serverItem);
                }
            }
        }
    }
}