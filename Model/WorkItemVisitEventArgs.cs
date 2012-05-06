using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ScrumPowerTools.Model
{
    internal class WorkItemVisitEventArgs : EventArgs
    {
        public WorkItem WorkItem { get; private set; }

        public WorkItemVisitEventArgs(WorkItem workItem)
        {
            WorkItem = workItem;
        }
    }
}