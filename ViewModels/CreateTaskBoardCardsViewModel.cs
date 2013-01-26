using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Framework.Presentation;
using ScrumPowerTools.Model;
using ScrumPowerTools.Packaging;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.ViewModels
{
    [Export(typeof(IMenuCommandHandler))]
    [HandlesMenuCommand(MenuCommands.CreateTaskBoardCards)]
    public class CreateTaskBoardCardsViewModel : IMenuCommandHandler
    {
        private readonly GeneralOptions options;
        private readonly WorkItemSelectionService workItemSelectionService;

        [ImportingConstructor]
        public CreateTaskBoardCardsViewModel(GeneralOptions options, WorkItemSelectionService workItemSelectionService)
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
            WorkItem[] workItems = workItemSelectionService.GetSelectedWorkItems();

            var workItemXml = new WorkItemXmlFileCreator();
            workItemXml.Create(workItems);

            string cardsFileName = Path.Combine(Path.GetDirectoryName(WorkItemXmlFileCreator.FileName), "Cards.html");

            using (Stream xsltStream = GetXsltStream())
            using (XmlReader xslt = XmlReader.Create(xsltStream))
            {
                var xslCompiledTransform = new XslCompiledTransform();
                xslCompiledTransform.Load(xslt);
                xslCompiledTransform.Transform(WorkItemXmlFileCreator.FileName, cardsFileName);
            }

            Process.Start(cardsFileName);
        }

        private Stream GetXsltStream()
        {
            if (File.Exists(options.TaskBoardCardsXsltFileName))
            {
                return new FileStream(options.TaskBoardCardsXsltFileName, FileMode.Open, FileAccess.Read);
            }

            if (options.TaskBoardCardsXsltFileName.StartsWith("$/"))
            {
                var teamProjectCollectionProvider = IoC.GetInstance<IVisualStudioAdapter>();
                TfsTeamProjectCollection tpc = teamProjectCollectionProvider.GetCurrent();

                var versionControlServer = tpc.GetService<VersionControlServer>();

                Item item = versionControlServer.GetItem(options.TaskBoardCardsXsltFileName, VersionSpec.Latest);

                return item.DownloadFile();
            }

            if (string.IsNullOrEmpty(options.TaskBoardCardsXsltFileName))
            {
                return Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("ScrumPowerTools.Resources.ScrumTaskBoardCards.xslt");
            }

            throw new ArgumentException("Unable to get XSLT file for creating taskboard cards.");
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
            var xmlWriter = XmlWriter.Create(FileName, new XmlWriterSettings { Indent = true });
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

            WriteFields(workItem, xmlWriter);

            xmlWriter.WriteEndElement();
        }

        private static void WriteFields(WorkItem workItem, XmlWriter xmlWriter)
        {
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

        public static string FileName
        {
            get { return Path.Combine(TempDirectory, "WorkItems.xml"); }
        }

        private static string TempDirectory
        {
            get { return Path.Combine(Path.GetTempPath(), "ScrumPowerToolsTaskBoardCards"); }
        }
    }
}