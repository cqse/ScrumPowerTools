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
			var foundationContextManager = Framework.Composition.IoC.GetInstance<IPackageServiceProvider>().GetService<ITeamFoundationContextManager>();
			var teamExplorer = Framework.Composition.IoC.GetInstance<IPackageServiceProvider>().GetService<ITeamExplorer>();

			var service = teamExplorer.CurrentPage.GetService<IWorkItemQueriesExt2>();
			var serviceProperties = service.GetType().GetProperties();
			if (serviceProperties.Count() != 2)
			{
				return null;
			}

			object items = null;
			string selectedQueryItemsPropertyName = "SelectedQueryItems";
			if (serviceProperties[0].Name.Equals(selectedQueryItemsPropertyName))
			{
				items = serviceProperties[0].GetValue(service);
			}
			else if (serviceProperties[1].Name.Equals(selectedQueryItemsPropertyName))
			{
				items = serviceProperties[1].GetValue(service);
			}

			var item = (items as QueryItem[])?.FirstOrDefault();
			if (item != null)
			{
				string path = string.Join("/", item.Path.Split('/').Skip(1));
				return new QueryPath(foundationContextManager.CurrentContext.TeamProjectName, path);
			}

			return null;
		}

		public override void ShowChangesetDetails(int changesetId)
			=> legacyAdapter.ShowChangesetDetails(changesetId);
	}
}