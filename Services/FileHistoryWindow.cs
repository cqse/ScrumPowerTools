using System;
using System.IO;
using EnvDTE;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace ScrumPowerTools.Services
{
    public class FileHistoryWindow
    {
        private readonly DTE dte;

        public FileHistoryWindow(DTE dte)
        {
            this.dte = dte;
        }

        public void Show(string path)
        {
            VersionControlExt vce = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;

            if (vce != null)
            {
                vce.History.Show(path, VersionSpec.Latest, 0, RecursionType.OneLevel);
            }
        }
    }
}