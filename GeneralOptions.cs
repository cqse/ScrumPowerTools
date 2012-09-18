using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using ScrumPowerTools.Packaging;
using System.Linq;

namespace ScrumPowerTools
{
    public class GeneralOptions : DialogPage
    {
        private const string MenuItems = "Menu Items";
        private const string MenuItemDescription = "Specifies if the menu item should be shown or not.";

        private readonly Dictionary<int, Func<bool>> commandVisibilityMapping;

        public GeneralOptions()
        {
            ShowAffectedChangesetFiles = MenuItemVisibility.Show;
            ShowChangesetsWithAffectedFiles = MenuItemVisibility.Show;
            Review = MenuItemVisibility.Show;
            TfsQueryShortcuts = new string[0];

            commandVisibilityMapping = new Dictionary<int,Func<bool>>
            {
                {MenuCommands.ShowAffectedChangesetFiles, () => ShowAffectedChangesetFiles == MenuItemVisibility.Show},
                {MenuCommands.ShowChangesetsWithAffectedFiles, () => ShowChangesetsWithAffectedFiles == MenuItemVisibility.Show},
                {MenuCommands.ShowReviewWindow, () => Review == MenuItemVisibility.Show}
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

        internal string[] TfsQueryShortcuts { get; set; }

        public bool IsEnabled(int commandId)
        {
            return commandVisibilityMapping[commandId]();
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            var package = (Package)GetService(typeof(Package));

            using (var registryKey = package.UserRegistryRoot)
            using (var settingsKey = registryKey.OpenSubKey(SettingsRegistryPath, true))
            {
                settingsKey.SetValue("TfsQueryShortcuts", TfsQueryShortcuts, RegistryValueKind.MultiString);
            }
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            var package = (Package)GetService(typeof(Package));

            using (var registryKey = package.UserRegistryRoot)
            using (var settingsKey = registryKey.OpenSubKey(SettingsRegistryPath))
            {
                if( (settingsKey != null)
                    && settingsKey.GetValueNames().Contains("TfsQueryShortcuts")
                    && settingsKey.GetValueKind("TfsQueryShortcuts") == RegistryValueKind.MultiString)
                {
                    TfsQueryShortcuts = (string[])settingsKey.GetValue("TfsQueryShortcuts", new string[0]);
                }
            }
        }
    }
}