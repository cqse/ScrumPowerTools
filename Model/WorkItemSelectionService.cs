using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace ScrumPowerTools.Model
{
    public class WorkItemSelectionService
    {
        private readonly DTE dte;
        private readonly DocumentService documentService;

        public WorkItemSelectionService(DTE dte, DocumentService documentService)
        {
            this.dte = dte;
            this.documentService = documentService;
        }

        public int GetFirstSelected()
        {
            Document activeDocument = dte.ActiveDocument;

            if ((activeDocument == null) || (documentService == null))
            {
                return 0;
            }

            string activeDocumentName = activeDocument.FullName;
            var resultsDocument = documentService.FindDocument(activeDocumentName, null) as IResultsDocument;

            if (resultsDocument != null)
            {
                return resultsDocument.SelectedItemIds.FirstOrDefault();
            }

            return 0;
        }

        public bool HasSelection()
        {
            return GetFirstSelected() != 0;
        }
    }
}