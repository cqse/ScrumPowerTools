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
            // Set the window title reading it from the resources.
            Caption = "Review";
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            //this.BitmapResourceID = 301;
            //this.BitmapIndex = 1;
            ToolBar = new CommandID(Identifiers.CommandGroupId, MenuCommands.TWToolbar);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            var view = ViewResolver.Resolve<ReviewViewModel>();

            viewModel = (view as FrameworkElement).DataContext as ReviewViewModel;
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
