using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.Framework.Composition;
using ScrumPowerTools.Framework.Extensions;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model.TaskBoardCards
{
    [Export]
    public class TaskBoardCardsModel
    {
        [ImportingConstructor]
        public TaskBoardCardsModel(GeneralOptions options, WorkItemSelectionService workItemSelectionService,
                                   IVisualStudioAdapter visualStudioAdapter)
        {
            this.options = options;
            this.workItemSelectionService = workItemSelectionService;
            this.visualStudioAdapter = visualStudioAdapter;
        }

        public void CreateCardsFromSelectedQuery()
        {
            var queryPath = visualStudioAdapter.GetCurrentSelectedQueryPath();
            var tpc = visualStudioAdapter.GetCurrent();
            var workItemStore = tpc.GetService<WorkItemStore>();

            WorkItemCollection workItems = workItemStore.GetWorkItems(queryPath);

            CreateCards(workItems.OfType<WorkItem>());
        }

        public void CreateCardsFromWorkItemSelection()
        {
            WorkItem[] workItems = workItemSelectionService.GetSelectedWorkItems();

            CreateCards(workItems);
        }

        private void CreateCards(IEnumerable<WorkItem> workItems)
        {
            EnsureTempDirectoryExists();

            var tpc = visualStudioAdapter.GetCurrent();
            var workItemStore = tpc.GetService<WorkItemStore>();

            WorkItem[] relatedWorkItems = workItems
                .SelectMany(wi => wi.WorkItemLinks.Cast<WorkItemLink>()
                    .Select(s => s.TargetId).Distinct()
                    .Select(id => workItemStore.GetWorkItem(id))).ToArray();

            new WorkItemXmlFileCreator().Create(workItems, relatedWorkItems, WorkItemsFileName);

            TransformWorkItemsToCards();

            OpenCreatedCardsInBrowser();
        }

        private void EnsureTempDirectoryExists()
        {
            Directory.CreateDirectory(TempDirectory);
        }

        private void TransformWorkItemsToCards()
        {
            using (Stream xsltStream = GetXsltStream())
            using (XmlReader xslt = XmlReader.Create(xsltStream))
            {
                var xslCompiledTransform = new XslCompiledTransform();
                xslCompiledTransform.Load(xslt);
                xslCompiledTransform.Transform(WorkItemsFileName, CardsFileName);

                CreateLocalCopyOfXslt(xsltStream);
            }
        }

        private static void OpenCreatedCardsInBrowser()
        {
            Process.Start(CardsFileName);
        }

        private Stream GetXsltStream()
        {
            if (File.Exists(options.TaskBoardCardsXsltFileName))
            {
                return new FileStream(options.TaskBoardCardsXsltFileName, FileMode.Open, FileAccess.Read);
            }

            if (IsXsltFromTfs)
            {
                var teamProjectCollectionProvider = IoC.GetInstance<IVisualStudioAdapter>();
                TfsTeamProjectCollection tpc = teamProjectCollectionProvider.GetCurrent();

                var versionControlServer = tpc.GetService<VersionControlServer>();

                Item item = versionControlServer.GetItem(options.TaskBoardCardsXsltFileName, VersionSpec.Latest);

                var memoryStream = new MemoryStream();

                using (var unseeakableFileStream = item.DownloadFile())
                {
                    unseeakableFileStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }

                return memoryStream;
            }

            if (String.IsNullOrEmpty(options.TaskBoardCardsXsltFileName))
            {
                return Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("ScrumPowerTools.Resources.TaskBoardCards.xslt");
            }

            throw new ArgumentException("Unable to get XSLT file for creating taskboard cards.");
        }

        private static void CreateLocalCopyOfXslt(Stream xsltStream)
        {
            using(var xsltLocalCopy = File.Create(XsltLocalCopyFileName))
            {
                xsltStream.Seek(0, SeekOrigin.Begin);
                xsltStream.CopyTo(xsltLocalCopy);
            }
        }

        private bool IsXsltFromTfs
        {
            get { return options.TaskBoardCardsXsltFileName.StartsWith("$/"); }
        }

        public bool CanCreateCardsFromSelection
        {
            get { return options.IsEnabled(Feature.TaskBoardCards) && workItemSelectionService.HasSelection(); }
        }

        public bool CanCreateCardsFromSelectedQuery
        {
            get { return options.IsEnabled(Feature.TaskBoardCards); }
        }

        private static string WorkItemsFileName
        {
            get { return Path.Combine(TempDirectory, "WorkItems.xml"); }
        }

        private static string XsltLocalCopyFileName
        {
            get { return Path.Combine(TempDirectory, "TaskBoardCards.xslt"); }
        }

        private static string CardsFileName
        {
            get { return Path.Combine(TempDirectory, "Cards.html"); }
        }

        private static string TempDirectory
        {
            get { return Path.Combine(Path.GetTempPath(), "ScrumPowerToolsTaskBoardCards"); }
        }

        private readonly GeneralOptions options;
        private readonly WorkItemSelectionService workItemSelectionService;
        private readonly IVisualStudioAdapter visualStudioAdapter;
    }
}