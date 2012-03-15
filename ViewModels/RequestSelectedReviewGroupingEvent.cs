using ScrumPowerTools.Framework.Presentation;

namespace ScrumPowerTools.ViewModels
{
    public class RequestSelectedReviewGroupingEvent : IMessage
    {
        public string Selection { get; set; }
    }
}