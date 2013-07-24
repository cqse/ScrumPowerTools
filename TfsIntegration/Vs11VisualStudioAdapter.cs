using System.Linq;
using EnvDTE;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.Extensibility;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.TfsIntegration
{
    internal class Vs11VisualStudioAdapter : IVisualStudioAdapter
    {
        private readonly DTE dte;

        public Vs11VisualStudioAdapter(DTE dte)
        {
            this.dte = dte;
        }

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

        public void ShowChangesetDetails(int changesetId)
        {
            TfsTeamProjectCollection tpc = GetCurrent();

            var teamExplorer = (ITeamExplorer)Package.GetGlobalService(typeof(ITeamExplorer));
            var versionControlServer = tpc.GetService<VersionControlServer>();
            Changeset changeset = versionControlServer.GetChangeset(changesetId);

            TeamExplorerUtils.Instance.TryNavigateToChangesetDetails(teamExplorer, changeset, TeamExplorerUtils.NavigateOptions.AlwaysNavigate);
        }

        public Workspace GetCurrentWorkSpace()
        {
            if (VersionControlExplorerExt != null)
            {
                return VersionControlExplorerExt.Explorer.Workspace;
            }

            return null;
        }

        private VersionControlExt VersionControlExplorerExt
        {
            get
            {
                return dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            }
        }
    }
}
