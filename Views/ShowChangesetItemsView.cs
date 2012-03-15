using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ScrumPowerTools.Model;
using ScrumPowerTools.Models;
using ScrumPowerTools.Packaging;

namespace ScrumPowerTools.Views
{
    public class ShowChangesetItemsView
    {
        private readonly DTE dte;

        public ShowChangesetItemsView(DTE dte)
        {
            this.dte = dte;
        }

        public void ConnectTo(ShowChangesetItemsModel model)
        {
            model.ShowChangesetItems += OnShowChangesetItems;
        }

        private void OnShowChangesetItems(object sender, ShowChangesetItemsEventArgs e)
        {
            var outWindow = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
            Guid id = Identifiers.ReviewOutputWindowId;

            outWindow.CreatePane(ref id, "Review", 1, 0);

            IVsOutputWindowPane generalPane;
            outWindow.GetPane(ref id, out generalPane);

            generalPane.Clear();


            var sortedServerItems = e.Paths.OrderBy(si => si).ToList();

            foreach (string serverItem in sortedServerItems)
            {
                generalPane.OutputString(serverItem + Environment.NewLine);
            }

            generalPane.OutputString(string.Format("{0} unique items." + Environment.NewLine, e.Paths.Count()));
            generalPane.Activate();

            dte.ExecuteCommand("View.Output", string.Empty);
        }
    }
}