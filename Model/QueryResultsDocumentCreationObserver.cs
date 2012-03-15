using System;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace ScrumPowerTools.Models
{
    /// <summary>
    /// Responsible for observing the creation of query results documents and notifying when it happens.
    /// </summary>
    public class QueryResultsDocumentCreationObserver
    {
        private readonly DocumentService documentService;

        public QueryResultsDocumentCreationObserver(DocumentService documentService)
        {
            this.documentService = documentService;

            ObserveDocumentCreations();
        }

        public event EventHandler<QueryResultsDocumentCreatedEventArgs> DocumentCreated = delegate { };

        private void ObserveDocumentCreations()
        {
            documentService.DocumentAdded += DocumentAdded;
        }

        void DocumentAdded(object sender, DocumentService.DocumentServiceEventArgs e)
        {
            var queryResultsDocument = e.Document as IResultsDocument;

            if ((queryResultsDocument != null) && (queryResultsDocument.QueryDocument == null))
            {
                DocumentCreated(this, new QueryResultsDocumentCreatedEventArgs(queryResultsDocument));
            }
        }
    }

    public class QueryResultsDocumentCreatedEventArgs : EventArgs
    {
        public QueryResultsDocumentCreatedEventArgs(IResultsDocument queryResultsDocument)
        {
            QueryResultsDocument = queryResultsDocument;
        }

        public IResultsDocument QueryResultsDocument { get; private set; }
    }
}