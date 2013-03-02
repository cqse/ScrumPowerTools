using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.ViewModels;


namespace ScrumPowerTools
{
    [Guid("1B3C001B-C023-42E0-AF06-24E0D6871EC8")]
    public sealed class ReviewToolWindow : ToolWindowPane
    {
        private readonly ReviewViewModel viewModel;

        public ReviewToolWindow() :
            base(null)
        {
            Caption = "Review";
            ToolBar = new CommandID(Identifiers.CommandGroupId, MenuCommands.TWToolbar);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            var view = ViewResolver.Resolve<ReviewViewModel>();

            viewModel = ((FrameworkElement)view).DataContext as ReviewViewModel;
            viewModel.PropertyChanged += ViewModelPropertyChanged;

            Content = view;
        }

        void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title")
            {
                Caption = viewModel.Title;
            }
        }
    }
}
