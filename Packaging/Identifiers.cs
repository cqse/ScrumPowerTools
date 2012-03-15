// Guids.cs
// MUST match guids.h
using System;

namespace ScrumPowerTools.Packaging
{
    internal static class Identifiers
    {
        /// <summary>
        /// The Id of the package.
        /// </summary>
        public const string PackageId = "534ed8ea-71db-4c8f-a27b-588c7e19d8f7";

        /// <summary>
        /// The grouping Id of the menu commands.
        /// </summary>
        public static readonly Guid CommandGroupId = new Guid("580831fe-ea0f-46f7-83d7-1d87039202cc");

        /// <summary>
        /// The Id of the output window which is used to show items for reviewing.
        /// </summary>
        public static readonly Guid ReviewOutputWindowId = new Guid("02DBE9C0-FC24-49D0-B7F2-E2B271979841");
    };
}