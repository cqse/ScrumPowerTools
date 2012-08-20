using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Services
{
    public class TfsQueryShortcutOpener
    {
        private readonly DocumentService documentService;
        private readonly ITeamProjectCollectionProvider teamProjectCollectionProvider;
        private readonly GeneralOptions options;

        public TfsQueryShortcutOpener(DocumentService documentService,
                                      ITeamProjectCollectionProvider teamProjectCollectionProvider, GeneralOptions options)
        {
            this.documentService = documentService;
            this.teamProjectCollectionProvider = teamProjectCollectionProvider;
            this.options = options;
        }

        public void Open()
        {
            try
            {
                //QueryPath queryPath = new QueryPath(@"$scrumpowertools/Team Queries/All Work Items");
                QueryPath queryPath = new QueryPath(options.TfsQueryShortcut);

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

        public void Assign()
        {
            var teamExplorer = IoC.GetInstance<IPackageServiceProvider>().GetService<IVsTeamExplorer>();

            IntPtr hierarchyPtr;
            uint selectedItemId;
            IVsMultiItemSelect dummy;
            teamExplorer.TeamExplorerWindow.GetCurrentSelection(out hierarchyPtr, out selectedItemId, out dummy);
            var hierarchy = (IVsHierarchy)Marshal.GetObjectForIUnknown(hierarchyPtr);
            Marshal.Release(hierarchyPtr);

            // now that we have the id and hierarchy, we can retrieve lots of properties about the node
            // in this case, we get the canonical name which is the in the form of (server/project/[query folders]*/query)    
            string canonicalName;    
            hierarchy.GetCanonicalName(selectedItemId, out canonicalName);    
            string[] tokens = canonicalName.Split('/');

            options.TfsQueryShortcut = "$" + string.Join("/", tokens.Skip(1));
            options.SaveSettingsToStorage();
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