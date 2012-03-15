namespace ScrumPowerTools.Framework.Presentation
{
    interface IHandle<in T> where T : IMessage
    {
        void Handle(T message);
    }
}