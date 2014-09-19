using System;
using ScrumPowerTools.Extensibility.Model;

namespace ScrumPowerTools.Model.Review
{
    public class ReviewItemModel : IReviewItem
    {
        public ReviewItemModel(ChangesetVisitEventArgs changesetVisitEventArgs)
        {
            ChangesetId = changesetVisitEventArgs.Changeset.ChangesetId;
            Comment = changesetVisitEventArgs.Changeset.Comment;
            CreationDate = changesetVisitEventArgs.Changeset.CreationDate;
            Committer = changesetVisitEventArgs.Committer;
        }

        public string Comment { get; private set; }

        public int ChangesetId { get; private set; }

        public DateTime CreationDate { get; private set; }

        public string Committer { get; private set; }

        public string ServerItem { get; internal set; }

        public string Change { get; internal set; }

        public string LocalFilePath { get; set; }
    }
}