using System;
using System.Collections.Generic;

namespace ScrumPowerTools.Model
{
    public class QuerySelectionTotalsEventArgs : EventArgs
    {
        public QuerySelectionTotalsEventArgs(IDictionary<string, double> totals)
        {
            Totals = totals;
        }

        public IDictionary<string, double> Totals { get; private set; }
    }
}