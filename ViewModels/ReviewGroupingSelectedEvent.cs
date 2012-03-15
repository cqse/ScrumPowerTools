using ScrumPowerTools.Framework.Presentation;

namespace ScrumPowerTools.ViewModels
{
    public class ReviewGroupingSelectedEvent : IMessage
    {
        public string Selection { get; set; }
    }
}