using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Extensibility.Service;

namespace ScrumPowerTools.ViewModels
{
    public class ShowReviewWindowMessage : IMessage
    {
        public int WorkItemId { get; set; }

        public IReviewItemFilter ReviewItemFilter { get; set; }

        public IReviewItemGlyphProvider ReviewItemGlyphProvider { get; set; }
    }
}