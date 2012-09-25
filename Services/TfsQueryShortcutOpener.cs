using System;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Services
{
    public class TfsQueryShortcutOpener
    {
        private readonly DocumentService documentService;
        private readonly IVisualStudioAdapter visualStudioAdapter;
        private readonly TfsQueryShortcutStore store;

        public TfsQueryShortcutOpener(DocumentService documentService,
                                      IVisualStudioAdapter visualStudioAdapter, TfsQueryShortcutStore store)
        {
            this.documentService = documentService;
            this.visualStudioAdapter = visualStudioAdapter;
            this.store = store;
        }

        public void Open(uint shortcutNr)
        {
            shortcutNr = shortcutNr & 0x0f;

            var queryPath = store.GetShortcut(shortcutNr);

            if (queryPath != null)
            {
                try
                {
                    QueryDefinition queryDefinition = GetQueryDefinition(queryPath);

                    if (queryDefinition != null)
                    {
                        ShowQueryResults(queryDefinition);
                    }
                }
                catch (Exception)
                {
                }   
            }
        }

        private QueryDefinition GetQueryDefinition(QueryPath queryPath)
        {
            var workItemStore = visualStudioAdapter.GetCurrent().GetService<WorkItemStore>();
            var queryHierarchy = workItemStore.Projects[queryPath.ProjectName].QueryHierarchy;

            var foundQueryItem = queryPath.PathNames
                .Aggregate<string, QueryItem>(queryHierarchy,
                    (queryItem, name) =>
                    {
                        var queryFolder = queryItem as QueryFolder;

                        if (queryFolder != null && queryFolder.Contains(name))
                        {
                            return queryFolder[name];
                        }

                        return null;
                    }
                );

            return foundQueryItem as QueryDefinition;
        }

        private void ShowQueryResults(QueryDefinition queryDefinition)
        {
            object lockToken = new object();
            IQueryDocument queryDocument = documentService.GetQuery(visualStudioAdapter.GetCurrent(),
                queryDefinition.Id.ToString(), lockToken);

            queryDocument.Load();
            IResultsDocument results = documentService.GetLinkResults(queryDocument, lockToken);

            documentService.ShowResults(results);

            results.Release(lockToken);
            queryDocument.Release(lockToken);
        }
    }
}