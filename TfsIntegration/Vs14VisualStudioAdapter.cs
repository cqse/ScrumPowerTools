using System;
using System.Linq;
using EnvDTE;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.Extensibility;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.TfsIntegration
{
	internal class Vs14VisualStudioAdapter : VisualStudioAdapterBase
	{
		private Vs11VisualStudioAdapter legacyAdapter;

		public Vs14VisualStudioAdapter(DTE dte)
			: base(dte)
		{
			this.legacyAdapter = new Vs11VisualStudioAdapter(dte);
		}

		public override TfsTeamProjectCollection GetCurrent()
			=> legacyAdapter.GetCurrent();

		public override QueryPath GetCurrentSelectedQueryPath()
			=> this.legacyAdapter.GetCurrentSelectedQueryPath();

		public override void ShowChangesetDetails(int changesetId)
			=> legacyAdapter.ShowChangesetDetails(changesetId);
	}
}