using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using ScrumPowerTools.Framework.Composition;

namespace ScrumPowerTools.TfsIntegration
{
    internal class Vs10TeamProjectCollectionProvider : ITeamProjectCollectionProvider
    {
        public TfsTeamProjectCollection GetCurrent()
        {
            var teamExplorer = IoC.GetInstance<IPackageServiceProvider>().GetService<IVsTeamExplorer>();
            
            return TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(teamExplorer.GetProjectContext().DomainUri));
        }
    }
}