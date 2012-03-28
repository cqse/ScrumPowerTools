using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using EnvDTE;
using ScrumPowerTools.Models;
using ScrumPowerTools.TfsIntegration;
using ScrumPowerTools.Views;

namespace ScrumPowerTools.Controllers
{
    public class QueryResultsTotalizerController
    {
        public QueryResultsTotalizerController(DocumentService docService, StatusBar statusBar, ITeamProjectCollectionProvider teamExplorer)
        {
            var documentCreationTracker = new QueryResultsDocumentCreationObserver(docService);

            documentCreationTracker.DocumentCreated += (sender, e) =>
            {
                var queryResultsModel = new QueryResultsTotalizerModel(teamExplorer);

                new QueryResultsDocumentSelectionObserver(e.QueryResultsDocument, queryResultsModel);

                new QueryResultsTotalizerView(queryResultsModel, statusBar);
            };
        }
    }
}