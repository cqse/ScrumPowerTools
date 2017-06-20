using System;
using System.IO;
using System.Windows;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model.Review;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ScrumPowerTools.Extensibility.Service;

namespace ScrumPowerTools.ViewModels
{
    public class ReviewItemViewModel : ViewModelBase
    {
        private Visibility visibility;
        private bool isGroupExpanded;
        private Visibility groupVisibility;

        public ReviewItemViewModel(ReviewItemModel reviewItemModel, IReviewItemGlyphProvider reviewItemGlyphProvider)
        {
            ChangesetId = reviewItemModel.ChangesetId;
            Comment = reviewItemModel.Comment;
            CreationDate = reviewItemModel.CreationDate;
            Committer = reviewItemModel.Committer;
            ServerItem = reviewItemModel.ServerItem;
            Change = reviewItemModel.Change;
            LocalFilePath = reviewItemModel.LocalFilePath;

            if (reviewItemGlyphProvider != null)
            {
                this.Glyph = reviewItemGlyphProvider.GetGlyph(reviewItemModel);
            }
        }

        public string Comment { get; private set; }

        public int ChangesetId { get; private set; }

        public DateTime CreationDate { get; private set; }

        public string Committer { get; private set; }

        public string ServerItem { get; internal set; }

        public string Change { get; internal set; }

        public string LocalFilePath { get; set; }

        public ImageSource Glyph { get; private set; }

        public string Folder
        {
            get { return Path.GetDirectoryName(LocalFilePath); }
        }

        public string Description
        {
            get
            {
                return GroupingType == ReviewGrouping.File
                    ? ServerItem
                    : Comment;
            }
        }

        public ReviewGrouping GroupingType { get; set; }

        public string Name
        {
            get { return Path.GetFileName(ServerItem); }
        }

        public void Exclude()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Include()
        {
            Visibility = Visibility.Visible;
        }

        public Visibility Visibility
        {
            get { return visibility; }
            set
            { 
                visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        public bool IsExcluded
        {
            get { return Visibility != Visibility.Visible; }
        }

        public bool IsGroupExpanded
        {
            get { return isGroupExpanded; }
            set
            {
                isGroupExpanded = value;
                NotifyOfPropertyChange(() => IsGroupExpanded);
            }
        }

        public Visibility GroupVisibility
        {
            get { return groupVisibility; }
            set
            {
                groupVisibility = value;
                NotifyOfPropertyChange(() => GroupVisibility);
            }
        }
    }
}