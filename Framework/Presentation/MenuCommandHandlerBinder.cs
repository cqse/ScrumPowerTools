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
                GetHandler(command.CommandID.ID).Execute(command.CommandID.ID);
            }
        }

        private static void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            if (null != command)
            {
                int commandId = command.CommandID.ID;
                var handler = GetHandler(commandId);
                command.Visible = handler.CanExecute(commandId);

                var menuTextProvider = handler as IProvideMenuText;
                if (menuTextProvider != null)
                {
                    command.Text = menuTextProvider.GetText(commandId);
                }
            }
        }

        private static IMenuCommandHandler GetHandler(int commandId)
        {
            var menuHandlers = IoC.GetInstances<IMenuCommandHandler>();
            foreach (IMenuCommandHandler handler in menuHandlers)
            {
                var handlesCommandAttributes =
                    (HandlesMenuCommandAttribute[])
                    handler.GetType().GetCustomAttributes(typeof(HandlesMenuCommandAttribute), false);

                if (handlesCommandAttributes.Any(a => a.CommandIdentifiers.Contains(commandId)))
                {
                    return handler;
                }
            }

            throw new ArgumentException("No menu handler registered for the specified id!");
        }
    }
}