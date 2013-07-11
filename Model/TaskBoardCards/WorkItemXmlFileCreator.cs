using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ScrumPowerTools.Model.TaskBoardCards
{
    internal class WorkItemXmlFileCreator
    {
        public void Create(IEnumerable<WorkItem> workItems, WorkItem[] relatedWorkItems, string fileName)
        {
            var xmlWriter = XmlWriter.Create(fileName, new XmlWriterSettings { Indent = true });
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("WorkItems");

            foreach (WorkItem workItem in workItems)
            {
                WriteWorkItemXml(workItem, xmlWriter, "WorkItem");
            }

            foreach (WorkItem workItem in relatedWorkItems)
            {
                WriteWorkItemXml(workItem, xmlWriter, "RelatedWorkItem");
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private static void WriteWorkItemXml(WorkItem workItem, XmlWriter xmlWriter, string elementName)
        {
            xmlWriter.WriteStartElement(elementName);
            xmlWriter.WriteAttributeString("Id", workItem.Id.ToString(CultureInfo.CurrentCulture));
            xmlWriter.WriteAttributeString("Type", workItem.Type.Name);

            WriteFields(workItem, xmlWriter);

            WriteWorkItemLinks(xmlWriter, workItem.WorkItemLinks);

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

        private static void WriteWorkItemLinks(XmlWriter xmlWriter, WorkItemLinkCollection workItemLinks)
        {
            xmlWriter.WriteStartElement("WorkItemLinks");

            foreach (WorkItemLink workItemLink in workItemLinks)
            {
                xmlWriter.WriteStartElement("WorkItemLink");

                xmlWriter.WriteAttributeString("LinkTypeEndName", workItemLink.LinkTypeEnd.Name);
                xmlWriter.WriteAttributeString("TargetId", workItemLink.TargetId.ToString());

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}