using System.ComponentModel.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Packaging;

namespace ScrumPowerTools.ViewModels
{
    [Export(typeof(IMenuCommandHandler))]
    [HandlesMenuCommand(MenuCommands.CreateScrumTaskBoardCards)]
    public class CreateScrumTaskBoardCardsViewModel : IMenuCommandHandler
    {
        public void Execute(int commandId)
        {
            CreateCards();
        }

        private void CreateCards()
        {
            //throw new NotImplementedException();
        }

        public bool CanExecute(int commandId)
        {
            return true;//options.IsEnabled(commandId) && workItemSelectionService.HasSelection();
        }
    }
}