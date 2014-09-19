// -----------------------------------------------------------------------
// <copyright file="IFileFilterProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ScrumPowerTools.Extensibility.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using ScrumPowerTools.Extensibility.Model;

    [Guid("99476C88-6167-46BE-9E53-8CB702EB37B4")]
    public interface IReviewItemFilter
    {
        bool IsIncluded(IReviewItem item);
    }
}
