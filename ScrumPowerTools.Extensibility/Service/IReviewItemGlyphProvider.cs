namespace ScrumPowerTools.Extensibility.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using ScrumPowerTools.Extensibility.Model;
    using System.Windows.Media;

    [Guid("4D340E0A-571D-4123-B89C-3CC91FC88EBD")]
    public interface IReviewItemGlyphProvider
    {
        /// <summary>
        /// Returns either null or an ImageSource with a glyph for the review item.
        /// </summary>
        ImageSource GetGlyph(IReviewItem item);
    }
}
