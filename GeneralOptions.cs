using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using ScrumPowerTools.Packaging;
using System.Linq;

namespace ScrumPowerTools
{
    public class GeneralOptions : DialogPage
    {
        private const string MenuItems = "Menu Items";
        private const string MenuItemDescription = "Specifies if the menu item should be shown or not.";

        private readonly Dictionary<uint, Func<bool>> commandVisibilityMapping;

        public GeneralOptions()
        {
            ShowAffectedChangesetFiles = MenuItemVisibility.Show;
            ShowChangesetsWithAffectedFiles = MenuItemVisibility.Show;
            Review = MenuItemVisibility.Show;


            commandVisibilityMapping = new Dictionary<uint,Func<bool>>
            {
                {MenuCommands.ShowAffectedChangesetFiles, () => { return ShowAffectedChangesetFiles == MenuItemVisibility.Show; }},
                {MenuCommands.ShowChangesetsWithAffectedFiles, () => { return ShowChangesetsWithAffectedFiles == MenuItemVisibility.Show; }},
                {MenuCommands.ShowReviewWindow, () => { return Review == MenuItemVisibility.Show; }}
            };
        }

        [Category(MenuItems)]
        [DisplayName(@"Show affected changeset files")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility ShowAffectedChangesetFiles { get; set; }

        [Category(MenuItems)]
        [DisplayName(@"Show changesets with affected files")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility ShowChangesetsWithAffectedFiles { get; set; }

        [Category(MenuItems)]
        [DisplayName(@"Review")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility Review { get; set; }

        public bool IsEnabled(uint commandId)
        {
            return commandVisibilityMapping[commandId]();
        }
    }
}