using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.ViewModels;

namespace ScrumPowerTools.Framework.Presentation
{
    public static class ComboBoxCommandHandlerBinder
    {
        public static void Bind(IMenuCommandService menuCommandService)
        {
            var menuHandlers = IoC.GetInstances<IComboBoxCommandHandler>();

            foreach (IComboBoxCommandHandler handler in menuHandlers)
            {
                var handlesComboBoxdAttributes = (HandlesComboBoxCommandAttribute[])handler.GetType().GetCustomAttributes(typeof(HandlesComboBoxCommandAttribute), false);

                foreach (HandlesComboBoxCommandAttribute attribute in handlesComboBoxdAttributes)
                {
                    var requestItemsCommandId = new CommandID(Identifiers.CommandGroupId, attribute.RequestItemsCommandId);
                    var menuItem = new OleMenuCommand(ProvideItemsCallback, requestItemsCommandId);
                    menuCommandService.AddCommand(menuItem);

                    var itemSelectionCommandId = new CommandID(Identifiers.CommandGroupId, attribute.SelectionCommandId);
                    var itemSelectionMenuItem = new OleMenuCommand(ItemSelectionCallback, itemSelectionCommandId);
                    menuCommandService.AddCommand(itemSelectionMenuItem);
                }
            }
        }

        private static void ProvideItemsCallback(object sender, EventArgs e)
        {
            var commandEvent = sender as MenuCommand;
            var commandEventArgs = e as OleMenuCmdEventArgs;

            if ((commandEvent != null) && (null != commandEventArgs))
            {
                if (commandEventArgs.Options == OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT)
                {
                    int commandId = commandEvent.CommandID.ID;
                    IComboBoxCommandHandler handler = ExecuteCommandOnRegisterdHandlers(commandId);
                    IEnumerable<string> items = handler.GetAvailableItems(commandId);

                    Marshal.GetNativeVariantForObject(items, commandEventArgs.OutValue);
                }
            }
        }

        private static void ItemSelectionCallback(object sender, EventArgs e)
        {
            var commandEvent = sender as MenuCommand;
            var commandEventArgs = e as OleMenuCmdEventArgs;

            if ((commandEvent != null) && (null != commandEventArgs))
            {
                int commandId = commandEvent.CommandID.ID;
                IComboBoxCommandHandler handler = ExecuteCommandOnRegisterdHandlers(commandId);

                if (commandEventArgs.OutValue != IntPtr.Zero)
                {
                    string items = handler.GetSelectedItem(commandId);

                    Marshal.GetNativeVariantForObject(items, commandEventArgs.OutValue);
                }
                else if (commandEventArgs.InValue != null)
                {
                    var selectedItem = commandEventArgs.InValue as string;

                    handler.Selected(selectedItem, commandId);
                }
            }
        }

        private static IComboBoxCommandHandler ExecuteCommandOnRegisterdHandlers(int commandId)
        {
            var handlers = IoC.GetInstances<IComboBoxCommandHandler>();

            foreach (IComboBoxCommandHandler menuCommandHandler in handlers)
            {
                var handlesCommandAttributes =
                    (HandlesComboBoxCommandAttribute[])
                    menuCommandHandler.GetType().GetCustomAttributes(typeof(HandlesComboBoxCommandAttribute), false);

                if (handlesCommandAttributes.Any(a => a.RequestItemsCommandId == commandId || a.SelectionCommandId == commandId))
                {
                    return menuCommandHandler;
                }
            }

            return null;
        }
    }
}