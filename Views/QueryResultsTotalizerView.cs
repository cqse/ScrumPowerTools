using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using ScrumPowerTools.Model;

namespace ScrumPowerTools.Views
{
    public class QueryResultsTotalizerView
    {
        private readonly StatusBar statusBar;

        public QueryResultsTotalizerView(QueryResultsTotalizerModel queryResultsModel, StatusBar statusBar)
        {
            this.statusBar = statusBar;
            queryResultsModel.QuerySelectionTotalsChanged += QuerySelectionTotalsChanged;
        }

        void QuerySelectionTotalsChanged(object sender, QuerySelectionTotalsEventArgs e)
        {
            ShowTotalsInStatusBar(e.Totals);
        }

        private void ShowTotalsInStatusBar(IDictionary<string, double> totals)
        {
            statusBar.Text = "Work Item Totals - " + string.Join(", ", totals.Select(k => k.Key + ": " + k.Value));
        }
    }
}