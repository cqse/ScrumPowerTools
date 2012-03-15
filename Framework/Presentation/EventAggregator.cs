using System.ComponentModel.Composition;
using System.Linq;
using ScrumPowerTools.Framework.Composition;

namespace ScrumPowerTools.Framework.Presentation
{
    [Export(typeof(EventAggregator))]
    public class EventAggregator
    {
        public void Publish<T>(T command) where T : IMessage
        {
            IoC.GetInstances<IHandle<T>>().ToList()
                .ForEach(x => x.Handle(command));
        }
    }
}