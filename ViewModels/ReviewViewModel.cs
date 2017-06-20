using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model.Review;
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
    [HandlesMenuCommand(MenuCommands.ClearExcludedItems)]
    public class ReviewViewModel : ViewModelBase,
                                   IHandle<ShowReviewWindowMessage>,
                                   IComboBoxCommandHandler,
                                   IMenuCommandHandler
    {
        public ReviewViewModel()
        {
            reviewItemViewModels = new ReviewItemViewModel[0];

            ReviewItems = new ListCollectionView(new List<ReviewItemViewModel>());
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
            get { return new DelegateCommand<ReviewItemViewModel>(SelectItem); }
        }

        public ICommand CompareWithPreviousVersionCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(CompareWithPreviousVersion); }
        }

        public ICommand CompareInitialVersionWithLatestChangeCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(CompareInitialVersionWithLatestChange); }
        }

        public ICommand ViewHistoryCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(ViewHistory); }
        }

        public ICommand ViewChangesetDetailsCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(ViewChangesetDetails, () => SelectedItem != null);
            }
        }

        public ICommand ExcludeCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(Exclude); }
        }

        public ICommand ExcludeChangesetCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(ExcludeChangeset, () => SelectedItem != null); }
        }

        public ICommand ExcludeFileCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(ExcludeFile, () => SelectedItem != null); }
        }

        public ICommand ShowItemCommand
        {
            get { return new DelegateCommand<ReviewItemViewModel>(ShowItem, () => true); }
        }

        public ReviewItemViewModel SelectedItem { get; set; }

        private void AssignColumns()
        {
            ObservableCollection<ColumnDescriptor> columns;

            if (SelectedGrouping == ReviewGrouping.File)
            {
                columns =
                    new ObservableCollection<ColumnDescriptor>(
                        AvailableColumns.Where(cd => cd.DisplayMember != "Name" && cd.DisplayMember != "Folder" && cd.DisplayMember != "Glyph"));
            }
            else
            {
                columns =
                    new ObservableCollection<ColumnDescriptor>(
                        AvailableColumns.Where(cd => cd.DisplayMember != "Comment"));
            }

            // TODO (MP) Remove Glyph if no glyph data available...

            Columns = columns;

            NotifyOfPropertyChange(() => Columns);
        }

        private void ShowItem(ReviewItemViewModel reviewItem)
        {
            if (SelectedGrouping == ReviewGrouping.File)
            {
                ShellDocumentOpener.Open(reviewItem.LocalFilePath);
            }
            else if (SelectedGrouping == ReviewGrouping.Changeset)
            {
                model.ShowChangesetDetails(reviewItem.ChangesetId);
            }
        }

        private void SelectItem(ReviewItemViewModel reviewItem)
        {
            ShellDocumentOpener.Open(reviewItem.LocalFilePath);
        }

        private void CompareWithPreviousVersion(ReviewItemViewModel reviewItem)
        {
            model.CompareWithPreviousVersion(reviewItem.ServerItem, reviewItem.ChangesetId);
        }

        private void CompareInitialVersionWithLatestChange(ReviewItemViewModel reviewItem)
        {
            model.CompareInitialVersionWithLatestChange(reviewItem.ServerItem);
        }

        private void ViewHistory(ReviewItemViewModel reviewItem)
        {
            TfsUiServices.ShowHistory(reviewItem.ServerItem);
        }

        private void ViewChangesetDetails(ReviewItemViewModel reviewItem)
        {
            model.ShowChangesetDetails(reviewItem.ChangesetId);
        }

        private void Exclude(ReviewItemViewModel reviewItem)
        {
            if (SelectedGrouping == ReviewGrouping.Changeset)
            {
                ExcludeChangeset(reviewItem);
            }
            else if (SelectedGrouping == ReviewGrouping.File)
            {
                ExcludeFile(reviewItem);
            }
        }

        private void ExcludeChangeset(ReviewItemViewModel reviewItem)
        {
            reviewItemViewModels
                .Where(r => r.ChangesetId == reviewItem.ChangesetId)
                .ToList()
                .ForEach(reviewItemViewModel => reviewItemViewModel.Exclude());

            SyncGroupVisibility();
        }

        private void ExcludeFile(ReviewItemViewModel reviewItem)
        {
            var fileItems = reviewItemViewModels.Where(r => r.LocalFilePath == reviewItem.LocalFilePath);

            foreach (ReviewItemViewModel reviewItemViewModel in fileItems)
            {
                reviewItemViewModel.Exclude();
            }

            SyncGroupVisibility();
        }

        private void SyncGroupVisibility()
        {
            IEnumerable<IGrouping<string, ReviewItemViewModel>> groups = new List<IGrouping<string, ReviewItemViewModel>>();

            if (SelectedGrouping == ReviewGrouping.Changeset)
            {
                groups = reviewItemViewModels.GroupBy(k => k.ChangesetId.ToString());
            }
            else if (SelectedGrouping == ReviewGrouping.File)
            {
                groups = reviewItemViewModels.GroupBy(k => k.LocalFilePath);                
            }

            foreach (IGrouping<string, ReviewItemViewModel> itemViewModels in groups)
            {
                bool areAllExcluded = itemViewModels.All(m => m.IsExcluded);

                foreach (ReviewItemViewModel reviewItemViewModel in itemViewModels)
                {
                    reviewItemViewModel.GroupVisibility = areAllExcluded ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        public void Handle(ShowReviewWindowMessage message)
        {
            model = new ReviewModel();

            model.Review(message.WorkItemId, message.ReviewItemFilter);

            Title = model.Title;

            reviewItemViewModels = model.ItemsToReview.Select(i => new ReviewItemViewModel(i, message.ReviewItemGlyphProvider)).ToArray();

            ReviewItems = new ListCollectionView(reviewItemViewModels);

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
                ExpandAllReviewItems(false);
            }
            else if (commandId == MenuCommands.ExpandAllReviewItems)
            {
                ExpandAllReviewItems(true);
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
            else if (commandId == MenuCommands.ClearExcludedItems)
            {
                ClearExcludedItems();
            }
        }

        private void ExpandAllReviewItems(bool expand)
        {
            reviewItemViewModels.ToList()
                .ForEach(ri => ri.IsGroupExpanded = expand);
        }

        private void ClearExcludedItems()
        {
            reviewItemViewModels.ToList().ForEach(reviewItemViewModel =>
            {
                reviewItemViewModel.Include();
                reviewItemViewModel.GroupVisibility = Visibility.Visible;
            });
        }

        public bool CanExecute(int commandId)
        {
            return true;
        }

        private void UpdateGrouping()
        {
            ReviewItems.GroupDescriptions.Clear();

            ReviewItems.SourceCollection.Cast<ReviewItemViewModel>()
                .ToList().ForEach(ri => ri.GroupingType = SelectedGrouping);

            if (SelectedGrouping == ReviewGrouping.File)
            {
                reviewItems.GroupDescriptions.Add(new PropertyGroupDescription("ServerItem"));
                ReviewItems.SortDescriptions.Add(new SortDescription("ServerItem", ListSortDirection.Ascending));
                ReviewItems.SortDescriptions.Add(new SortDescription("ChangesetId", ListSortDirection.Ascending));
            }
            else
            {
                reviewItems.GroupDescriptions.Add(new PropertyGroupDescription("ChangesetId"));
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
                    new ColumnDescriptor
                    {
                        HeaderText = "",
                        CellTemplate = "imageTemplate"
                    },
                    new ColumnDescriptor
                    {
                        HeaderText = "Changeset",
                        DisplayMember = "ChangesetId"
                    },
                    new ColumnDescriptor
                    {
                        HeaderText = "Name",
                        DisplayMember = "Name"
                    },
                    new ColumnDescriptor
                    {
                        HeaderText = "Folder",
                        DisplayMember = "Folder"
                    },
                    new ColumnDescriptor
                    {
                        HeaderText = "Comment",
                        DisplayMember = "Comment"
                    },
                    new ColumnDescriptor
                    {
                        HeaderText = "Change",
                        DisplayMember = "Change"
                    },
                    new ColumnDescriptor
                    {
                        HeaderText = "Committer",
                        DisplayMember = "Committer"
                    },
                    new ColumnDescriptor
                    {
                        HeaderText = "Creation date",
                        DisplayMember = "CreationDate"
                    }
                };
            }
        }

        public Visibility IsGroupedByFile
        {
            get
            {
                return (SelectedGrouping == ReviewGrouping.File)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public Visibility IsGroupedByChangeset
        {
            get
            {
                return (SelectedGrouping == ReviewGrouping.Changeset)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private string title;

        [Import(typeof(IToolWindowActivator))]
        private IToolWindowActivator ToolWindowActivator { get; set; }

        [Import(typeof(ShellDocumentOpener))]
        private ShellDocumentOpener ShellDocumentOpener { get; set; }

        [Import(typeof(TfsUiServices))]
        private TfsUiServices TfsUiServices { get; set; }

        private ListCollectionView reviewItems;
        private ReviewModel model;
        private ReviewItemViewModel[] reviewItemViewModels;

        private ReviewGrouping SelectedGrouping { get; set; }
    }

    public enum ReviewGrouping
    {
        Changeset = 0,
        File = 1
    }
}