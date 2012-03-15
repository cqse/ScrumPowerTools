using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ScrumPowerTools.Model;
using ScrumPowerTools.Models;
using ScrumPowerTools.Packaging;

namespace ScrumPowerTools.Views
{
    public class ShowChangesetsView
    {
        private readonly DTE dte;
        private IVsOutputWindowPane generalPane;

        public ShowChangesetsView(DTE dte)
        {
            this.dte = dte;
        }

        public void ConnectTo(ShowChangesetsModel model)
        {
            model.ShowChangesets += OnShowChangesets;
        }

        private void OnShowChangesets(object sender, ShowChangesetsEventArgs e)
        {
            var outWindow = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
            Guid id = Identifiers.ReviewOutputWindowId;

            outWindow.CreatePane(ref id, "Review", 1, 0);

            outWindow.GetPane(ref id, out generalPane);

            generalPane.Clear();

            ShowChangesets(e.Changesets);

            generalPane.Activate();

            dte.ExecuteCommand("View.Output", string.Empty);
        }

        private void ShowChangesets(IEnumerable<ChangesetModel> changesets)
        {
            var sortedChangesets = changesets.OrderBy(c => c.CreationDate);

            foreach (var changesetModel in sortedChangesets)
            {
                ShowChangeset(changesetModel);
            }
        }

        private void ShowChangeset(ChangesetModel changesetModel)
        {
            generalPane.OutputString(string.Format("Changeset {0}, {1}, {2}" + Environment.NewLine,
                changesetModel.Number, changesetModel.CreationDate, changesetModel.Committer));
            generalPane.OutputString(changesetModel.Title + Environment.NewLine);

            var sortedServerItems = changesetModel.ServerItems.OrderBy(si => si).ToList();

            foreach (string serverItem in sortedServerItems)
            {
                generalPane.OutputString(serverItem + Environment.NewLine);
            }

            generalPane.OutputString(Environment.NewLine);
        }
    }
}