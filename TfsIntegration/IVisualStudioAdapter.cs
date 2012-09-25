using Microsoft.TeamFoundation.Client;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.TfsIntegration
{
    public interface IVisualStudioAdapter
    {
        /// <summary>
        /// Returns the uri of the current tfs project
        /// </summary>
        TfsTeamProjectCollection GetCurrent();

        /// <summary>
        /// Returns the <see cref="QueryPath"/> of the currently selected team query node.
        /// </summary>
        /// <returns></returns>
        QueryPath GetCurrentSelectedQueryPath();
    }
}
