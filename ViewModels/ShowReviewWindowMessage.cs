using ScrumPowerTools.Framework.Presentation;

namespace ScrumPowerTools.ViewModels
{
    public class ShowReviewWindowMessage : IMessage
    {
        public int WorkItemId { get; set; }
    }
}