using System;

namespace ScrumPowerTools.Framework.Presentation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class HandlesComboBoxCommandAttribute : Attribute
    {
        private readonly int requestItemsCommandId;
        private readonly int selectionCommandId;

        public HandlesComboBoxCommandAttribute(uint requestItemsCommandId, uint selectionCommandId)
        {
            this.requestItemsCommandId = (int)requestItemsCommandId;
            this.selectionCommandId = (int)selectionCommandId;
        }

        public int RequestItemsCommandId { get { return requestItemsCommandId; } }

        public int SelectionCommandId { get { return selectionCommandId; } }
    }
}