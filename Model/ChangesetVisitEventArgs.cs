using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace ScrumPowerTools.Models
{
    public class ChangesetVisitEventArgs : EventArgs
    {
        public ChangesetVisitEventArgs(Changeset changeset, IEnumerable<string> localizedServerItems, Workspace workspace)
        {
            LocalizedServerItems = localizedServerItems;
            Workspace = workspace;
            Changeset = changeset;
            Committer = changeset.Committer;
        }

        public IEnumerable<string> LocalizedServerItems { get; private set; }

        public Workspace Workspace { get; private set; }

        public Changeset Changeset { get; private set; }

        public string Committer { get; private set; }
    }
}