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
        private readonly ITeamProjectCollectionProvider teamProjectCollectionProvider;

        public TfsQueryShortcutOpener(DocumentService documentService,
                                      ITeamProjectCollectionProvider teamProjectCollectionProvider)
        {
            this.documentService = documentService;
            this.teamProjectCollectionProvider = teamProjectCollectionProvider;
        }

        public void Open()
        {
            try
            {
                QueryPath queryPath = new QueryPath(@"$scrumpowertools/Team Queries/All Work Items");

                QueryDefinition queryDefinition = GetQueryDefinition(queryPath);

                if(queryDefinition != null)
                {
                    ShowQueryResults(queryDefinition);
                }
            }
            catch (Exception)
            {
            }
        }

        private void ShowQueryResults(QueryDefinition queryDefinition)
        {
            object lockToken = new object();
            IQueryDocument queryDocument = documentService.GetQuery(teamProjectCollectionProvider.GetCurrent(),
                queryDefinition.Id.ToString(), lockToken);

            queryDocument.Load();
            IResultsDocument results = documentService.GetLinkResults(queryDocument, lockToken);

            documentService.ShowResults(results);

            results.Release(lockToken);
            queryDocument.Release(lockToken);
        }

        private QueryDefinition GetQueryDefinition(QueryPath queryPath)
        {
            var workItemStore = teamProjectCollectionProvider.GetCurrent().GetService<WorkItemStore>();
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
    }

    public class QueryPath
    {
        public QueryPath(string queryPath)
        {
            var queryPathElements = queryPath.Split('/');

            if (!queryPathElements.Any())
            {
                throw new ArgumentException("The specified path is not valid."); // no elements
            }

            ProjectName = queryPathElements.First().TrimStart('$');

            PathNames = queryPathElements.Skip(1).ToArray();
        }

        public string ProjectName { get; private set; }
        public string[] PathNames { get; private set; }
    }
}