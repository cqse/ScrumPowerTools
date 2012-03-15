using System;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ScrumPowerTools.Models
{
    internal class WorkItemVisitor
    {
        public event EventHandler<WorkItemVisitEventArgs> WorkItemVisit = delegate { }; 

        private readonly WorkItemStore workItemStore;


        public WorkItemVisitor(WorkItemStore workItemStore)
        {
            this.workItemStore = workItemStore;
        }


        public void Visit(WorkItem workItem)
        {
            WorkItemVisit(this, new WorkItemVisitEventArgs(workItem));

            VisitChildren(workItem);
        }

        private void VisitChildren(WorkItem workItem)
        {
            foreach (WorkItemLink link in workItem.WorkItemLinks)
            {
                if (link.LinkTypeEnd.IsForwardLink && link.LinkTypeEnd.Name == "Child")
                {
                    WorkItem childWorkItem = workItemStore.GetWorkItem(link.TargetId);

                    Visit(childWorkItem);
                }
            }
        }
    }
}