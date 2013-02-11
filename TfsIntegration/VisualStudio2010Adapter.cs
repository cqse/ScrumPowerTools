using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.TfsIntegration
{
    internal class VisualStudio2010Adapter : IVisualStudioAdapter
    {
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
    }
}