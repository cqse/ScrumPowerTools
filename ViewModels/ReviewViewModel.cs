using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.ViewModels
{
    [Export(typeof(ReviewViewModel))]
    [Export(typeof(IHandle<ShowReviewWindowMessage>))]
    [Export(typeof(IComboBoxCommandHandler))]
    [HandlesComboBoxCommand(MenuCommands.FillReviewGroupingComboList, MenuCommands.ChangeReviewGrouping)]
    [Export(typeof(IMenuCommandHandler))]
    [HandlesMenuCommand(MenuCommands.CollapseAllReviewItems, MenuCommands.ExpandAllReviewItems)]
    [HandlesMenuCommand(MenuCommands.CompareWithPreviousVersion, MenuCommands.CompareWithVersionBeforeFirstChange)]
    public class ReviewViewModel : ViewModelBase,
        IHandle<ShowReviewWindowMessage>,
        IComboBoxCommandHandler,
        IMenuCommandHandler
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

        public ICommand CompareInitialVersionWithLatestChangeCommand
        {
            get { return new DelegateCommand<ReviewItemModel>(CompareInitialVersionWithLatestChange); }
        }

        public ICommand ViewHistoryCommand
        {
            get
            {
                return new DelegateCommand<ReviewItemModel>(ViewHistory,
                    () => SelectedItem != null);
            }
        }

        public ICommand ViewChangesetDetailsCommand
        {
            get
            {
                return new DelegateCommand<ReviewItemModel>(ViewChangesetDetails,
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
            model.CompareWithPreviousVersion(reviewItem.ServerItem, reviewItem.ChangesetId);
        }

        private void CompareInitialVersionWithLatestChange(ReviewItemModel reviewItem)
        {
            model.CompareInitialVersionWithLatestChange(reviewItem.ServerItem);
        }

        private void ViewHistory(ReviewItemModel reviewItem)
        {
            TfsUiServices.ShowHistory(reviewItem.ServerItem);
        }

        private void ViewChangesetDetails(ReviewItemModel reviewItem)
        {
            TfsUiServices.ShowChangesetDetails(reviewItem.ChangesetId);
        }

        public void Handle(ShowReviewWindowMessage message)
        {
            model = new ReviewModel();
            model.Review(message.WorkItemId);

            Title = model.Title;
            ReviewItems = new ListCollectionView(model.ItemsToReview.ToList());
            
            UpdateGrouping();
            ToolWindowActivator.Activate<ReviewToolWindow>();
        }

        public IEnumerable<string> GetAvailableItems(int commandId)
        {
            return Enum.GetNames(typeof(ReviewGrouping));
        }

        public string GetSelectedItem(int commandId)
        {
            return Enum.GetName(typeof(ReviewGrouping), SelectedGrouping);
        }

        public void Selected(string selectedItem, int commandId)
        {
            SelectedGrouping = (ReviewGrouping)Enum.Parse(typeof(ReviewGrouping), selectedItem);

            AssignColumns();

            UpdateGrouping();
        }

        public void Execute(int commandId)
        {
            if (commandId == MenuCommands.CollapseAllReviewItems)
            {
                IsExpanded = false;
            }
            else if (commandId == MenuCommands.ExpandAllReviewItems)
            {
                IsExpanded = true;
            }
            else if (commandId == MenuCommands.CompareWithPreviousVersion)
            {
                if (SelectedItem != null)
                {
                    model.CompareWithPreviousVersion(SelectedItem.ServerItem, SelectedItem.ChangesetId);                    
                }
            }
            else if (commandId == MenuCommands.CompareWithVersionBeforeFirstChange)
            {
                if (SelectedItem != null)
                {
                    model.CompareInitialVersionWithLatestChange(SelectedItem.ServerItem);
                }
            }
        }

        public bool CanExecute(int commandId)
        {
            return true;
        }

        private void UpdateGrouping()
        {
            ReviewItems.GroupDescriptions.Clear();

            if (SelectedGrouping == ReviewGrouping.File)
            {
                reviewItems.GroupDescriptions.Add(new PropertyGroupDescription("ServerItem"));
                ReviewItems.SortDescriptions.Add(new SortDescription("ServerItem", ListSortDirection.Ascending));
                ReviewItems.SortDescriptions.Add(new SortDescription("ChangesetId", ListSortDirection.Ascending));
            }
            else
            {
                reviewItems.GroupDescriptions.Add(new PropertyGroupDescription("Comment"));
                ReviewItems.SortDescriptions.Add(new SortDescription("ChangesetId", ListSortDirection.Ascending));
                ReviewItems.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
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

        [Import(typeof(TfsUiServices))]
        private TfsUiServices TfsUiServices { get; set; }

        private ListCollectionView reviewItems;
        private ReviewModel model;

        private ReviewGrouping SelectedGrouping { get; set; }

        private enum ReviewGrouping
        {
            Changeset = 0,
            File = 1
        }
    }
}