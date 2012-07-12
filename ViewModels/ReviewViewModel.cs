using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.ViewModels
{
    [Export(typeof(ReviewViewModel))]
    [Export(typeof(IHandle<ShowReviewWindowMessage>))]
    [Export(typeof(IHandle<RequestReviewGroupingChoicesEvent>))]
    [Export(typeof(IHandle<RequestSelectedReviewGroupingEvent>))]
    [Export(typeof(IHandle<ReviewGroupingSelectedEvent>))]
    [Export(typeof(IHandle<CollapseAllReviewItemsEvent>))]
    [Export(typeof(IHandle<ExpandAllReviewItemsEvent>))]
    public class ReviewViewModel : ViewModelBase,
        IHandle<ShowReviewWindowMessage>,
        IHandle<RequestReviewGroupingChoicesEvent>,
        IHandle<RequestSelectedReviewGroupingEvent>,
        IHandle<ReviewGroupingSelectedEvent>,
        IHandle<CollapseAllReviewItemsEvent>,
        IHandle<ExpandAllReviewItemsEvent>
    {
        public ReviewViewModel()
        {
            ReviewItems = new ListCollectionView(new List<ReviewItemModel>());
            SelectedGrouping = ReviewGrouping.Changeset;

            AssignColumns();
        }

        public ListCollectionView ReviewItems
        {
            get { return reviewItems; }
            private set
            {
                reviewItems = value;
                NotifyOfPropertyChange(() => ReviewItems);
            }
        }

        public string Title
        {
            get { return title; }
            set 
            { 
                title = value; 
                NotifyOfPropertyChange(() => Title);
            }
        }

        public ObservableCollection<ColumnDescriptor> Columns { get; private set; }

        public ICommand SelectItemCommand
        {
            get { return new DelegateCommand<ReviewItemModel>(SelectItem); }
        }

        public ICommand CompareWithPreviousVersionCommand
        {
            get { return new DelegateCommand<ReviewItemModel>(CompareWithPreviousVersion); }
        }

        public ICommand ViewHistoryCommand
        {
            get
            {
                return new DelegateCommand<ReviewItemModel>(ViewHistory,
                    () => SelectedItem != null);
            }
        }

        public bool IsExpanded 
        { 
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }
        }

        public ReviewItemModel SelectedItem { get; set; }

        private void AssignColumns()
        {
            ObservableCollection<ColumnDescriptor> columns;

            if(SelectedGrouping == ReviewGrouping.File)
            {
                columns = new ObservableCollection<ColumnDescriptor>(AvailableColumns.Where(cd => cd.DisplayMember != "Name" && cd.DisplayMember != "Folder"));                
            }
            else
            {
                columns = new ObservableCollection<ColumnDescriptor>(AvailableColumns.Where(cd => cd.DisplayMember != "Comment"));
            }

            Columns = columns;

            NotifyOfPropertyChange(() => Columns);
        }

        private void SelectItem(ReviewItemModel reviewItem)
        {
            ShellDocumentOpener.Open(reviewItem.LocalFilePath);
        }

        private void CompareWithPreviousVersion(ReviewItemModel reviewItem)
        {
            var differentiator = new TfsItemDifferentiator();
            differentiator.CompareWithPreviousVersion(reviewItem.ServerItem, reviewItem.ChangesetId);
        }

        private void ViewHistory(ReviewItemModel reviewItem)
        {
            FileHistoryWindow.Show(reviewItem.ServerItem);
        }

        public void Handle(ShowReviewWindowMessage message)
        {
            var reviewModel = new ReviewModel();
            reviewModel.Review(message.WorkItemId);

            Title = reviewModel.Title;
            ReviewItems = new ListCollectionView(reviewModel.ItemsToReview.ToList());
            
            UpdateGrouping();
            ToolWindowActivator.Activate<ReviewToolWindow>();
        }

        public void Handle(RequestReviewGroupingChoicesEvent message)
        {
            message.Choices = Enum.GetNames(typeof(ReviewGrouping));
        }

        public void Handle(RequestSelectedReviewGroupingEvent message)
        {
            message.Selection = Enum.GetName(typeof(ReviewGrouping), SelectedGrouping);
        }

        public void Handle(ReviewGroupingSelectedEvent message)
        {
            SelectedGrouping = (ReviewGrouping)Enum.Parse(typeof(ReviewGrouping), message.Selection);

            AssignColumns();

            UpdateGrouping();
        }

        public void Handle(CollapseAllReviewItemsEvent message)
        {
            IsExpanded = false;
        }

        public void Handle(ExpandAllReviewItemsEvent message)
        {
            IsExpanded = true;
        }

        private void UpdateGrouping()
        {
            ReviewItems.GroupDescriptions.Clear();

            if (SelectedGrouping == ReviewGrouping.File)
            {
                reviewItems.GroupDescriptions.Add(new PropertyGroupDescription("ServerItem"));
            }
            else
            {
                reviewItems.GroupDescriptions.Add(new PropertyGroupDescription("Comment"));
            }
        }

        private IEnumerable<ColumnDescriptor> AvailableColumns
        {
            get
            {
                return new List<ColumnDescriptor>
                {
                    new ColumnDescriptor { HeaderText = "Changeset", DisplayMember = "ChangesetId" },
                    new ColumnDescriptor { HeaderText = "Name", DisplayMember = "Name" },
                    new ColumnDescriptor { HeaderText = "Folder", DisplayMember = "Folder" },
                    new ColumnDescriptor { HeaderText = "Comment", DisplayMember = "Comment" },
                    new ColumnDescriptor { HeaderText = "Change", DisplayMember = "Change" },
                    new ColumnDescriptor { HeaderText = "Committer", DisplayMember = "Committer" },
                    new ColumnDescriptor { HeaderText = "CreationDate", DisplayMember = "CreationDate" }
                };
            }
        }

        private string title;

        private bool isExpanded;

        [Import(typeof(IToolWindowActivator))]
        private IToolWindowActivator ToolWindowActivator { get; set; }

        [Import(typeof(ShellDocumentOpener))]
        private ShellDocumentOpener ShellDocumentOpener { get; set; }

        [Import(typeof(FileHistoryWindow))]
        private FileHistoryWindow FileHistoryWindow { get; set; }

        private ListCollectionView reviewItems;

        private ReviewGrouping SelectedGrouping { get; set; }

        private enum ReviewGrouping
        {
            Changeset = 0,
            File = 1
        }
    }
}