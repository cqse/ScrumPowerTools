using EnvDTE;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Interfaces;
using ScrumPowerTools.Model;
using ScrumPowerTools.Models;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.ViewModels;
using ScrumPowerTools.Views;

namespace ScrumPowerTools.Controllers
{
    public class MenuCommandController
    {
        private readonly DTE dte;
        private readonly DocumentService docService;
        private readonly ITeamProjectUriProvider teamProjectUriProvider;

        public MenuCommandController(DTE dte, DocumentService docService, ITeamProjectUriProvider teamProjectUriProvider)
        {
            this.dte = dte;
            this.docService = docService;
            this.teamProjectUriProvider = teamProjectUriProvider;
        }

        public bool Execute(uint commandId)
        {
            if (commandId == MenuCommands.ShowAffectedChangesetFiles)
            {
                var model = new ShowChangesetItemsModel(dte, docService, teamProjectUriProvider);
                var view = new ShowChangesetItemsView(dte);

                view.ConnectTo(model);

                model.Execute();

                return true;
            }
            else if (commandId == MenuCommands.ShowChangesetsWithAffectedFiles)
            {
                var model = new ShowChangesetsModel(dte, docService, teamProjectUriProvider);
                var view = new ShowChangesetsView(dte);

                view.ConnectTo(model);

                model.Execute();

                return true;
            }
            else if (commandId == MenuCommands.ShowReviewWindow)
            {
                var workItemSelectionService = IoC.GetInstance<WorkItemSelectionService>();

                if (workItemSelectionService.HasSelection())
                {
                    IoC.GetInstance<EventAggregator>().Publish(new ShowReviewWindowMessage()
                    {
                        WorkItemId = workItemSelectionService.GetFirstSelected()                                               
                    });
                }
            }

            return false;
        }

        public bool CanExecute(uint commandId)
        {
            return new WorkItemSelectionService(dte, docService).HasSelection();
        }
    }
}