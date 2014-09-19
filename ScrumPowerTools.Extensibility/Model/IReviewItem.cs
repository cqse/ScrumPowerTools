// -----------------------------------------------------------------------
// <copyright file="IReviewItem.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ScrumPowerTools.Extensibility.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;

    [Guid("8D28C81D-15A7-44B5-BD1B-0D1DC792F538")]
    public interface IReviewItem
    {
        string Comment { get; }

        int ChangesetId { get; }

        DateTime CreationDate { get; }

        string Committer { get; }

        string ServerItem { get; }

        string Change { get; }

        string LocalFilePath { get; }
    }
}
