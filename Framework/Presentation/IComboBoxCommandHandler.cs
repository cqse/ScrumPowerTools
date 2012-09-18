using System.Collections.Generic;

namespace ScrumPowerTools.Framework.Presentation
{
    public interface IComboBoxCommandHandler
    {
        IEnumerable<string> GetAvailableItems(int commandId);

        string GetSelectedItem(int commandId);

        void Selected(string selectedItem, int commandId);
    }
}