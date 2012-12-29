using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
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
            var workItems = workItemSelectionService.GetSelectedWorkItems();

            var workItemXml = new WorkItemXmlFileCreator();
            workItemXml.Create(workItems);
        }

        public bool CanExecute(int commandId)
        {
            return options.IsEnabled(commandId) && workItemSelectionService.HasSelection();
        }
    }

    internal class WorkItemXmlFileCreator
    {
        public void Create(WorkItem[] workItems)
        {
            EnsureTempDirectoryExists();

            CreateXmlFile(workItems);
        }

        private void CreateXmlFile(WorkItem[] workItems)
        {
            var xmlWriter = XmlWriter.Create(FileName);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("WorkItems");

            foreach (WorkItem workItem in workItems)
            {
                WriteWorkItemXml(workItem, xmlWriter);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private static void WriteWorkItemXml(WorkItem workItem, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("WorkItem");
            xmlWriter.WriteAttributeString("Id", workItem.Id.ToString(CultureInfo.CurrentCulture));
            xmlWriter.WriteAttributeString("Type", workItem.Type.Name);
            xmlWriter.WriteStartElement("Fields");

            foreach (Field field in workItem.Fields)
            {
                WriteFieldXml(xmlWriter, field);
            }

            xmlWriter.WriteEndElement();
        }

        private static void WriteFieldXml(XmlWriter xml, Field field)
        {
            xml.WriteStartElement("Field");
            xml.WriteAttributeString("RefName", field.ReferenceName);
            xml.WriteAttributeString("Value", field.Value != null ? field.Value.ToString() : "");
            xml.WriteEndElement();
        }

        private void EnsureTempDirectoryExists()
        {
            Directory.CreateDirectory(TempDirectory);            
        }

        private string FileName
        {
            get { return Path.Combine(TempDirectory, "WorkItems.xml"); }
        }

        private string TempDirectory
        {
            get { return Path.Combine(Path.GetTempPath(), "ScrumPowerToolsTaskBoardCards"); }
        }
    }
}