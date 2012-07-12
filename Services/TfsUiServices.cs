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
                VersionControlExt.History.Show(path, VersionSpec.Latest, 0, RecursionType.OneLevel);
            }
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
                return dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            }
        }
    }
}