using System;
using System.IO;
using ScrumPowerTools.Models;

namespace ScrumPowerTools.Model
{
    public class ReviewItemModel
    {
        public ReviewItemModel(ChangesetVisitEventArgs changesetVisitEventArgs)
        {
            ChangesetId = changesetVisitEventArgs.Changeset.ChangesetId;
            Comment = changesetVisitEventArgs.Changeset.Comment;
            CreationDate = changesetVisitEventArgs.Changeset.CreationDate;
            Committer = changesetVisitEventArgs.Committer;
        }

        public string Comment { get; internal set; }

        public int ChangesetId { get; internal set; }

        public DateTime CreationDate { get; internal set; }

        public string Committer { get; internal set; }

        public string ServerItem { get; internal set; }

        public string Change { get; internal set; }

        public string LocalFilePath { get; set; }

        public string Folder
        {
            get { return Path.GetDirectoryName(LocalFilePath); }
        }

        public string Name
        {
            get { return Path.GetFileName(ServerItem); }
        }
    }
}