using EnvDTE;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace ScrumPowerTools.Services
{
    public class TfsUiServices
    {
        private readonly DTE dte;

        public TfsUiServices(DTE dte)
        {
            this.dte = dte;
        }

        public void ShowHistory(string path)
        {
            if (VersionControlExt != null)
            {
                var history = VersionControlExt.History.Show(path, VersionSpec.Latest, 0, RecursionType.Full);
            }
        }

        private VersionControlExt VersionControlExt
        {
            get
            {
                return dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            }
        }
    }
}