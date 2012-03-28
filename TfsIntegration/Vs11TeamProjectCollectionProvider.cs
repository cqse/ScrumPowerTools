using Microsoft.TeamFoundation.Client;
using ScrumPowerTools.Framework.Composition;

namespace ScrumPowerTools.TfsIntegration
{
    internal class Vs11TeamProjectCollectionProvider : ITeamProjectCollectionProvider
    {
        public TfsTeamProjectCollection GetCurrent()
        {
            var teamExplorer = IoC.GetInstance<IPackageServiceProvider>().GetService<ITeamFoundationContextManager>();
            return teamExplorer.CurrentContext.TeamProjectCollection;
        }
    }
}
