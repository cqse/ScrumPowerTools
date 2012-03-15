using System.Collections.Generic;
using ScrumPowerTools.Framework.Presentation;

namespace ScrumPowerTools.ViewModels
{
    public class RequestReviewGroupingChoicesEvent : IMessage
    {
        public IEnumerable<string> Choices { get; set; }
    }
}