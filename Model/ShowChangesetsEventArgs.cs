using System;
using System.Collections.Generic;

namespace ScrumPowerTools.Model
{
    internal class ShowChangesetsEventArgs : EventArgs
    {
        public IEnumerable<ChangesetModel> Changesets { get; private set; }

        internal ShowChangesetsEventArgs(IEnumerable<ChangesetModel> changesets)
        {
            Changesets = changesets;
        }
    }
}