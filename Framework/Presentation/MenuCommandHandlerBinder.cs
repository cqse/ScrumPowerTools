using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Packaging;

namespace ScrumPowerTools.Framework.Presentation
{
    public static class MenuCommandHandlerBinder
    {
        public static void Bind(IMenuCommandService menuCommandService)
        {
            var menuHandlers = IoC.GetInstances<IMenuCommandHandler>();

            foreach (IMenuCommandHandler menuCommandHandler in menuHandlers)
            {
                var handlesCommandAttributes = (HandlesMenuCommandAttribute[])menuCommandHandler.GetType().GetCustomAttributes(typeof(HandlesMenuCommandAttribute), false);

                IEnumerable<int> coomandIdentifiers = handlesCommandAttributes.SelectMany(attr => attr.CommandIdentifiers);

                foreach (int commandId in coomandIdentifiers)
                {
                    var menuCommandId = new CommandID(Identifiers.CommandGroupId, commandId);
                    var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandId);
                    menuItem.BeforeQueryStatus += OnBeforeQueryStatus;

                    menuCommandService.AddCommand(menuItem);
                }
            }
        }

        private static void MenuItemCallback(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            if (null != command)
            {
                ExecuteCommandOnRegisterdHandlers(command.CommandID.ID, handler => handler.Execute(command.CommandID.ID));
            }
        }

        private static void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            if (null != command)
            {
                ExecuteCommandOnRegisterdHandlers(command.CommandID.ID,
                    handler => command.Visible = handler.CanExecute(command.CommandID.ID));
            }
        }

        private static void ExecuteCommandOnRegisterdHandlers(int commandId, Action<IMenuCommandHandler> commandAction)
        {
            var menuHandlers = IoC.GetInstances<IMenuCommandHandler>();
            foreach (IMenuCommandHandler menuCommandHandler in menuHandlers)
            {
                var handlesCommandAttributes =
                    (HandlesMenuCommandAttribute[])
                    menuCommandHandler.GetType().GetCustomAttributes(typeof(HandlesMenuCommandAttribute), false);

                if (handlesCommandAttributes.Any(a => a.CommandIdentifiers.Contains(commandId)))
                {
                    commandAction(menuCommandHandler);
                }
            }
        }
    }
}