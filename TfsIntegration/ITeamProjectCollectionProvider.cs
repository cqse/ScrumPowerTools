using Microsoft.TeamFoundation.Client;

namespace ScrumPowerTools.TfsIntegration
{
    public interface ITeamProjectCollectionProvider
    {
        /// <summary>
        /// Returns the uri of the current tfs project
        /// </summary>
        TfsTeamProjectCollection GetCurrent();
    }
}
