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
using System.Reflection;

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
			// TODO (VZ): implement
			throw new NotImplementedException();

			var foundationContextManager = (ITeamFoundationContextManager)Package.GetGlobalService(typeof(ITeamFoundationContextManager));

			object tEobj = Package.GetGlobalService(typeof(ITeamExplorer));
			dynamic tE = Package.GetGlobalService(typeof(ITeamExplorer));
			dynamic service;

			Type tEtype = tE.GetType();
			PropertyInfo[] pi = tEtype.GetProperties();

			var x = ((Microsoft.TeamFoundation.Controls.ITeamExplorer)tE).CurrentPage;

			//var teamExplorer = (ITeamExplorer)Package.GetGlobalService(typeof(ITeamExplorer));
			//var service = teamExplorer.CurrentPage.GetService<IWorkItemQueriesExt2>();

			var q = service.SelectedQueryIds.FirstOrDefault();
			if (q != null)
			{

			}

			//QueryItem query = service.SelectedQueryItems.FirstOrDefault();

			//if (query != null)
			//{
			//	string path = string.Join("/", query.Path.Split('/').Skip(1));

			//	return new QueryPath(foundationContextManager.CurrentContext.TeamProjectName, path);
			//}

			return null;
		}

		public override void ShowChangesetDetails(int changesetId)
			=> legacyAdapter.ShowChangesetDetails(changesetId);
	}
}