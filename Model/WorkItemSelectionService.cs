using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using ScrumPowerTools.Framework.Extensions;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    public class WorkItemSelectionService
    {
        private readonly DTE dte;
        private readonly DocumentService documentService;
        private readonly IVisualStudioAdapter visualStudioAdapter;

        public WorkItemSelectionService(DTE dte, DocumentService documentService, IVisualStudioAdapter visualStudioAdapter)
        {
            this.dte = dte;
            this.documentService = documentService;
            this.visualStudioAdapter = visualStudioAdapter;
        }

        public int GetFirstSelected()
        {
            return GetSelectedWorkItemIdentifiers().FirstOrDefault();
        }

        public bool HasSelection()
        {
            return GetSelectedWorkItemIdentifiers().Any();
        }

        public WorkItemCollection GetSelectedWorkItems()
        {
            var workItemStore = visualStudioAdapter.GetCurrent().GetService<WorkItemStore>();

            return workItemStore.GetWorkItems(GetSelectedWorkItemIdentifiers());
        }

        private int[] GetSelectedWorkItemIdentifiers()
        {
            Document activeDocument = dte.ActiveDocument;

            if ((activeDocument == null) || (documentService == null))
            {
                return new int[0];
            }

            string activeDocumentName = activeDocument.FullName;

            IWorkItemTrackingDocument workItemTrackingDocument = documentService.FindDocument(activeDocumentName, null);
            var workItemDocument = workItemTrackingDocument as IWorkItemDocument;
            var resultsDocument = workItemTrackingDocument as IResultsDocument;

            if (resultsDocument != null)
            {
                return resultsDocument.SelectedItemIds;
            }

            if ((workItemDocument != null) && (workItemDocument.Item != null))
            {
                return new [] {workItemDocument.Item.Id};
            }

            return new int[0];
        }
    }
}