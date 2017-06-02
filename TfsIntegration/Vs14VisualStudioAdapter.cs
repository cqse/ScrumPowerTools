using System;
using EnvDTE;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using ScrumPowerTools.Services;
using System.Reflection;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.Extensibility;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.Framework.Composition;

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
		{
			ITeamFoundationContextManager foundationContextManager =
				IoC.GetInstance<IPackageServiceProvider>().GetService<ITeamFoundationContextManager>();
			ITeamExplorer teamExplorer =
				IoC.GetInstance<IPackageServiceProvider>().GetService<ITeamExplorer>();

			IWorkItemQueriesExt2 service = teamExplorer.CurrentPage.GetService<IWorkItemQueriesExt2>();
			Guid id = service.SelectedQueryIds.FirstOrDefault();

			if (id == null)
			{
				return null;
			}

			WorkItemStore wis = legacyAdapter.GetCurrent().GetService<WorkItemStore>();

			QueryDefinition def = wis.GetQueryDefinition(id);
			return new QueryPath(def.Project.Name, def.Path);

			// TODO (VZ): Error handling and rethink return value
		}

		public override void ShowChangesetDetails(int changesetId)
			=> legacyAdapter.ShowChangesetDetails(changesetId);
	}
}