using System;
using System.Collections.Generic;

namespace ScrumPowerTools.Model
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