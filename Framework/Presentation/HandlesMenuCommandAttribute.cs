using System;

namespace ScrumPowerTools.Framework.Presentation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class HandlesMenuCommandAttribute : Attribute
    {
        private readonly int[] commandIdentifiers;

        public HandlesMenuCommandAttribute(params int[] commandIdentifiers)
        {
            this.commandIdentifiers = commandIdentifiers;
        }

        public int[] CommandIdentifiers { get { return commandIdentifiers; } }
    }
}