using System;
using System.Collections.Generic;

namespace ScrumPowerTools.Models
{
    public class ShowChangesetItemsEventArgs : EventArgs
    {
        public ShowChangesetItemsEventArgs(IEnumerable<string> serverItems)
        {
            Paths = serverItems;
        }

        public IEnumerable<string> Paths { get; private set; }
    }
}