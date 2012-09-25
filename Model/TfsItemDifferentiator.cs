using System;
using Microsoft.TeamFoundation.VersionControl.Client;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    internal class TfsItemDifferentiator
    {
        private readonly VersionControlServer versionControlServer;

        public TfsItemDifferentiator()
        {
            var tpc = IoC.GetInstance<IVisualStudioAdapter>().GetCurrent();

            versionControlServer = tpc.GetService<VersionControlServer>();
        }

        public void CompareWithPreviousVersion(string serverItem, int changesetId)
        {
            var previousVersionId = GetPreviousChangesetId(serverItem, changesetId);
            Compare(serverItem, previousVersionId, changesetId);
        }

        public void CompareInitialVersionWithLatestChange(string serverItem, int firstChangesetId, int lastChangesetId)
        {
            var initialChangesetId = GetPreviousChangesetId(serverItem, firstChangesetId);
            Compare(serverItem, initialChangesetId, lastChangesetId);
        }

        private void Compare(string serverItem, int fromChangesetId, int toChangesetId)
        {
            try
            {
                var itemFrom = new DiffItemVersionedFile(versionControlServer, serverItem,
                    new ChangesetVersionSpec(fromChangesetId));
                var itemTo = new DiffItemVersionedFile(versionControlServer, serverItem,
                    new ChangesetVersionSpec(toChangesetId));

                Difference.VisualDiffItems(versionControlServer, itemFrom, itemTo);
            }
            catch (Exception ex)
            {
                //HACK
            }
        }

        private int GetPreviousChangesetId(string serverItem, int changesetId)
        {
            VersionSpec version = new ChangesetVersionSpec(changesetId);

            var itemChangesetHistory = versionControlServer.QueryHistory(serverItem, version, 0, RecursionType.Full,
                null, null,
                version, 2, false, false);

            int previousVersionId = changesetId;
            foreach (Changeset changeset in itemChangesetHistory)
            {
                previousVersionId = changeset.ChangesetId;
            }
            return previousVersionId;
        }
    }
}