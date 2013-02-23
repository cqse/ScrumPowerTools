using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ScrumPowerTools.Model.TaskBoardCards
{
    internal class WorkItemXmlFileCreator
    {
        public void Create(IEnumerable<WorkItem> workItems, string fileName)
        {
            var xmlWriter = XmlWriter.Create(fileName, new XmlWriterSettings { Indent = true });
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
    }
}