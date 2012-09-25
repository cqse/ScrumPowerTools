using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.Extensibility;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.TfsIntegration
{
    internal class Vs11VisualStudioAdapter : IVisualStudioAdapter
    {
        public TfsTeamProjectCollection GetCurrent()
        {
            var teamExplorer = IoC.GetInstance<IPackageServiceProvider>().GetService<ITeamFoundationContextManager>();

            return teamExplorer.CurrentContext.TeamProjectCollection;
        }

        public QueryPath GetCurrentSelectedQueryPath()
        {
            var foundationContextManager = (ITeamFoundationContextManager)Package.GetGlobalService(typeof(ITeamFoundationContextManager));
            var teamExplorer = (ITeamExplorer)Package.GetGlobalService(typeof(ITeamExplorer));
            var service = teamExplorer.CurrentPage.GetService<IWorkItemQueriesExt>();

            QueryItem query = service.SelectedQueryItems.FirstOrDefault();

            if (query != null)
            {
                string path = string.Join("/", query.Path.Split('/').Skip(1));

                return new QueryPath(foundationContextManager.CurrentContext.TeamProjectName, path);
            }

            return null;
        }
    }
}
