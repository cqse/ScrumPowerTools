using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Shell.Interop;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.TfsIntegration
{
    internal class Vs10TeamProjectCollectionProvider : ITeamProjectCollectionProvider
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