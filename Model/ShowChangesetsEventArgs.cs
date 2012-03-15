using System;
using System.Collections.Generic;
using ScrumPowerTools.Model;

namespace ScrumPowerTools.Models
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