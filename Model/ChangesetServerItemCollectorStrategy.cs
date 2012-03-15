using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.Models;

namespace ScrumPowerTools.Model
{
    internal interface IWorkItemCollectorStrategy
    {
        void Collect(WorkItem workItem, WorkItemStore store, VersionControlServer versionControlServer);
    }

    internal class ChangesetServerItemCollectorStrategy : IWorkItemCollectorStrategy
    {
        public IEnumerable<string> ServerItems { get; private set; }

        private IList<string> serverItems;

        public void Collect(WorkItem workItem, WorkItemStore store, VersionControlServer versionControlServer)
        {
            var changesetVisitor = new ChangesetVisitor(store, versionControlServer);
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