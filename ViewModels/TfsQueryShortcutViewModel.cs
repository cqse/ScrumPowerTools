using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.ViewModels
{
    [Export(typeof(IMenuCommandHandler))]
    [HandlesMenuCommand(MenuCommands.AssignTfsQueryShortcut1, MenuCommands.AssignTfsQueryShortcut2)]
    [HandlesMenuCommand(MenuCommands.AssignTfsQueryShortcut3, MenuCommands.AssignTfsQueryShortcut4, MenuCommands.AssignTfsQueryShortcut5)]
    [HandlesMenuCommand(MenuCommands.OpenTfsQuery1, MenuCommands.OpenTfsQuery2)]
    [HandlesMenuCommand(MenuCommands.OpenTfsQuery3, MenuCommands.OpenTfsQuery4, MenuCommands.OpenTfsQuery5)]
    public class TfsQueryShortcutViewModel : IMenuCommandHandler
    {
        private readonly Dictionary<int, Action> commandHandlerMappings;
            
        [ImportingConstructor]
        public TfsQueryShortcutViewModel(TfsQueryShortcutAssigner shortcutAssigner, TfsQueryShortcutOpener shortcutOpener)
        {
            commandHandlerMappings = new Dictionary<int, Action>
            {
                {MenuCommands.AssignTfsQueryShortcut1, () => shortcutAssigner.Assign(0)},
                {MenuCommands.AssignTfsQueryShortcut2, () => shortcutAssigner.Assign(1)},
                {MenuCommands.AssignTfsQueryShortcut3, () => shortcutAssigner.Assign(2)},
                {MenuCommands.AssignTfsQueryShortcut4, () => shortcutAssigner.Assign(3)},
                {MenuCommands.AssignTfsQueryShortcut5, () => shortcutAssigner.Assign(4)},
                {MenuCommands.OpenTfsQuery1, () => shortcutOpener.Open(0)},
                {MenuCommands.OpenTfsQuery2, () => shortcutOpener.Open(1)},
                {MenuCommands.OpenTfsQuery3, () => shortcutOpener.Open(2)},
                {MenuCommands.OpenTfsQuery4, () => shortcutOpener.Open(3)},
                {MenuCommands.OpenTfsQuery5, () => shortcutOpener.Open(4)},
            };
        }

        public void Execute(int commandId)
        {
            commandHandlerMappings[commandId]();
        }

        public bool CanExecute(int commandId)
        {
            return true;
        }
    }
}