namespace ScrumPowerTools.Framework.Presentation
{
    public interface IMenuCommandHandler
    {
        void Execute(int commandId);
        bool CanExecute(int commandId);
    }
}