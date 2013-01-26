using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System.Linq;
using MenuCommands = ScrumPowerTools.Packaging.MenuCommands;

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
            ShowCreateTaskBoardCards = MenuItemVisibility.Show;
            Review = MenuItemVisibility.Show;
            TfsQueryShortcuts = new string[0];
            TaskBoardCardsXsltFileName = "";

            commandVisibilityMapping = new Dictionary<int,Func<bool>>
            {
                {MenuCommands.ShowAffectedChangesetFiles, () => ShowAffectedChangesetFiles == MenuItemVisibility.Show},
                {MenuCommands.ShowChangesetsWithAffectedFiles, () => ShowChangesetsWithAffectedFiles == MenuItemVisibility.Show},
                {MenuCommands.ShowReviewWindow, () => Review == MenuItemVisibility.Show},
                {MenuCommands.CreateTaskBoardCards, () => ShowCreateTaskBoardCards == MenuItemVisibility.Show}
            };
        }

        [Category(MenuItems)]
        [DisplayName("Show affected changeset files")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility ShowAffectedChangesetFiles { get; set; }

        [Category(MenuItems)]
        [DisplayName("Show changesets with affected files")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility ShowChangesetsWithAffectedFiles { get; set; }

        [Category(MenuItems)]
        [DisplayName("Review")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility Review { get; set; }

        [Category(MenuItems)]
        [DisplayName("Assign work item query shortcut")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility AssignWorkItemQueryShortcut { get; set; }

        [Category(MenuItems)]
        [DisplayName("Show create task board cards")]
        [Description(MenuItemDescription)]
        public MenuItemVisibility ShowCreateTaskBoardCards { get; set; }

        [Category("Task Board Cards")]
        [DisplayName("Task board cards xslt file")]
        [Description("Specifies which XSLT file to use for creating the task board cards. The default will be used when not specified.")]
        [EditorAttribute(typeof(XsltFileNameEditor), typeof(UITypeEditor))]
        public string TaskBoardCardsXsltFileName { get; set; }

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

        private class XsltFileNameEditor : FileNameEditor
        {
            protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
            {
                openFileDialog.Filter = "XSLT Files|*.xslt;*.xsl|All Files (*.*)|*.*";
            }
        }
    }


}