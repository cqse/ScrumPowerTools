using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Services;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace ScrumPowerTools.TfsIntegration
{
    internal class Vs10VisualStudioAdapter : IVisualStudioAdapter
    {
        private readonly EnvDTE.DTE dte;

        public Vs10VisualStudioAdapter(EnvDTE.DTE dte)
        {
            this.dte = dte;
        }

        public TfsTeamProjectCollection GetCurrent()
        {
            var teamExplorer = IoC.GetInstance<IPackageServiceProvider>().GetService<IVsTeamExplorer>();
            
            return TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(teamExplorer.GetProjectContext().DomainUri));
        }

        public QueryPath GetCurrentSelectedQueryPath()
        {        
            var teamExplorer = IoC.GetInstance<IPackageServiceProvider>().GetService<IVsTeamExplorer>();
            var canonicalName = GetSelectedCanonicalNameFromTeamExplorer(teamExplorer);

            string[] tokens = canonicalName.Split('/');
            string projectName = tokens.Skip(1).First();
            string teamQuery = string.Join("/", tokens.Skip(2));

            return new QueryPath(projectName, teamQuery);
        }

        public void ShowChangesetDetails(int changesetId)
        {
            if (VersionControlExt != null)
            {
                VersionControlExt.ViewChangesetDetails(changesetId);
            }
        }

        private VersionControlExt VersionControlExt
        {
            get
            {
                var dte = IoC.GetInstance<IPackageServiceProvider>().GetService<EnvDTE.DTE>();

                if (dte != null)
                {
                    return
                        dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as
                        VersionControlExt;
                }

                return null;
            }
        }

        private static string GetSelectedCanonicalNameFromTeamExplorer(IVsTeamExplorer teamExplorer)
        {
            IntPtr hierarchyPtr;
            uint selectedItemId;
            IVsMultiItemSelect dummy;
            teamExplorer.TeamExplorerWindow.GetCurrentSelection(out hierarchyPtr, out selectedItemId, out dummy);
            var hierarchy = (IVsHierarchy)Marshal.GetObjectForIUnknown(hierarchyPtr);
            Marshal.Release(hierarchyPtr);

            string canonicalName;
            hierarchy.GetCanonicalName(selectedItemId, out canonicalName);

            return canonicalName;
        }

        // TODO (MP) extract to common base class? Also consider getting the workspace from pending changes window?

        public Workspace GetCurrentWorkSpace()
        {
            Workspace workSpace = null;

            if (VersionControlExplorerExt != null)
            {
                workSpace = VersionControlExplorerExt.SolutionWorkspace ?? VersionControlExplorerExt.Explorer.Workspace;
            }

            if (workSpace == null)
            {
                throw new Exception("Unable te get the workspace.\nPlease open the solution of the items you want to review or go to the Source Control Explorer to initialize the workspace.");
            }

            return workSpace;
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