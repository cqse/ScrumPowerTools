using System.ComponentModel.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model.TaskBoardCards;
using ScrumPowerTools.Packaging;

namespace ScrumPowerTools.ViewModels
{
	[Export(typeof(IMenuCommandHandler))]
	[HandlesMenuCommand(MenuCommands.CreateTaskBoardCards)]
	[HandlesMenuCommand(MenuCommands.CreateTaskBoardCardsForQueryResult)]
	public class TaskBoardCardsViewModel : IMenuCommandHandler
	{
		private readonly TaskBoardCardsModel model;

		[ImportingConstructor]
		public TaskBoardCardsViewModel(TaskBoardCardsModel model)
		{
			this.model = model;
		}

		public void Execute(int commandId)
		{
			if (commandId == MenuCommands.CreateTaskBoardCards)
			{
				model.CreateCardsFromWorkItemSelection();
			}
			else if (commandId == MenuCommands.CreateTaskBoardCardsForQueryResult)
			{
				model.CreateCardsFromSelectedQuery();
			}
		}

		public bool CanExecute(int commandId)
		{
			if (commandId == MenuCommands.CreateTaskBoardCards)
			{
				return model.CanCreateCardsFromSelection;
			}

			if (commandId == MenuCommands.CreateTaskBoardCardsForQueryResult)
			{
				return model.CanCreateCardsFromSelectedQuery;
			}

			return false;
		}
	}
}