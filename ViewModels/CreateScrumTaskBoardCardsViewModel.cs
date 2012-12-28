using System.ComponentModel.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model;
using ScrumPowerTools.Packaging;

namespace ScrumPowerTools.ViewModels
{
    [Export(typeof(IMenuCommandHandler))]
    [HandlesMenuCommand(MenuCommands.CreateScrumTaskBoardCards)]
    public class CreateScrumTaskBoardCardsViewModel : IMenuCommandHandler
    {
        private readonly GeneralOptions options;
        private readonly WorkItemSelectionService workItemSelectionService;

        [ImportingConstructor]
        public CreateScrumTaskBoardCardsViewModel(GeneralOptions options, WorkItemSelectionService workItemSelectionService)
        {
            this.options = options;
            this.workItemSelectionService = workItemSelectionService;
        }

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
            return options.IsEnabled(commandId) && workItemSelectionService.HasSelection();
        }
    }
}