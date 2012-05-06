using System;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace ScrumPowerTools.Model
{
    public class QueryResultsDocumentSelectionObserver
    {
        private readonly IResultsDocument queryResultsDocument;
        private readonly QueryResultsTotalizerModel queryResultsModel;

        public QueryResultsDocumentSelectionObserver(IResultsDocument queryResultsDocument, QueryResultsTotalizerModel queryResultsModel)
        {
            this.queryResultsDocument = queryResultsDocument;
            this.queryResultsModel = queryResultsModel;

            queryResultsDocument.Loaded += (sender, e) => queryResultsModel.RefreshWorkItems(queryResultsDocument);
            queryResultsDocument.Reloaded += (sender, e) => queryResultsModel.RefreshWorkItems(queryResultsDocument);
            queryResultsDocument.Saved += (s, e) => queryResultsModel.RefreshWorkItems(queryResultsDocument);

            queryResultsDocument.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            queryResultsModel.RefreshTotals((queryResultsDocument.SelectedItemIds ?? new int[0]));
        }
    }
}