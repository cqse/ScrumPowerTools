namespace ScrumPowerTools.TfsIntegration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using EnvDTE;
	using Microsoft.TeamFoundation.VersionControl.Client;
	using Microsoft.VisualStudio.TeamFoundation.VersionControl;

	internal abstract class VisualStudioAdapterBase : IVisualStudioAdapter
	{
		protected DTE dte { get; private set; }

		public VisualStudioAdapterBase(DTE dte)
		{
			this.dte = dte;
		}

		public Workspace GetCurrentWorkSpace()
		{
			Workspace workSpace = null;

			if (VersionControlExplorerExt != null)
			{
				workSpace = VersionControlExplorerExt.SolutionWorkspace ?? VersionControlExplorerExt.Explorer.Workspace;
			}

			if (workSpace == null)
			{
				throw new Exception("Unable te get the workspace, please open the solution of the items you want to review or go to the Source Control Explorer to initialize the workspace.");
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

		public abstract Microsoft.TeamFoundation.Client.TfsTeamProjectCollection GetCurrent();

		public abstract Services.QueryPath GetCurrentSelectedQueryPath();

		public abstract void ShowChangesetDetails(int changesetId);
	}
}