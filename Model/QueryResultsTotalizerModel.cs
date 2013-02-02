using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using ScrumPowerTools.Framework.Extensions;
using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Model
{
    public class QueryResultsTotalizerModel
    {
        private readonly IVisualStudioAdapter visualStudioAdapter;
        private readonly string[] excludedFieldNames = {"ID", "Stack Rank", "Priority"};

        public QueryResultsTotalizerModel(IVisualStudioAdapter visualStudioAdapter)
        {
            this.visualStudioAdapter = visualStudioAdapter;

            CurrentWorkItems = new WorkItem[0];
            NumericFieldDefinitions = new FieldDefinition[0];
        }

        public event EventHandler<QuerySelectionTotalsEventArgs> QuerySelectionTotalsChanged = delegate {};

        public void RefreshWorkItems(IResultsDocument queryResultsDocument)
        {
            string query = queryResultsDocument.QueryDocument.QueryText;
            Hashtable context = GetTfsQueryParameters(queryResultsDocument);

            var tpc = visualStudioAdapter.GetCurrent();
            var workItemStore = tpc.GetService<WorkItemStore>();

            var workItemQuery = new Query(workItemStore, query, context);
            
            NumericFieldDefinitions = GetNumericFieldDefinitions(workItemQuery);

            if (!NumericFieldDefinitions.Any())
            {
                CurrentWorkItems = new WorkItem[0];
                return;
            }

            WorkItemCollection workItemCollection = workItemStore.GetWorkItems(workItemQuery, NumericFieldDefinitions);

            WorkItem[] workItemsA = new WorkItem[workItemCollection.Count];
            ((ICollection)workItemCollection).CopyTo(workItemsA, 0);

            CurrentWorkItems = workItemsA;

            RefreshTotals((queryResultsDocument.SelectedItemIds ?? new int[0]));
        }

        private IEnumerable<FieldDefinition> NumericFieldDefinitions { get; set; }

        public void RefreshTotals(int[] selectedItemIds)
        {
            var workItemsToTotalize = CurrentWorkItems;

            if (selectedItemIds.Length > 1)
            {
                workItemsToTotalize = CurrentWorkItems.Where(w => selectedItemIds.Contains(w.Id)).ToArray();
            }

            var totalsPerField = GetTotalsPerField(workItemsToTotalize);

            RemoveZeroValues(totalsPerField);

            QuerySelectionTotalsChanged(this, new QuerySelectionTotalsEventArgs(totalsPerField));
        }

        protected WorkItem[] CurrentWorkItems { get; set; }

        private IEnumerable<FieldDefinition> GetNumericFieldDefinitions(Query workItemInfoQuery)
        {
            return (from FieldDefinition displayField in workItemInfoQuery.DisplayFieldList
                    where IsNumericField(displayField) && !excludedFieldNames.Contains(displayField.Name)
                    select displayField).ToArray();
        }

        private static Hashtable GetTfsQueryParameters(IResultsDocument queryResultsDocument)
        {
            return new Hashtable()
            {
                { "project", queryResultsDocument.TeamProject }
            };
        }

        private static bool IsNumericField(FieldDefinition fieldDefinition)
        {
            return ((fieldDefinition.FieldType == FieldType.Double) || (fieldDefinition.FieldType == FieldType.Integer));
        }

        private Dictionary<string, double> GetTotalsPerField(WorkItem[] workItems)
        {
            var totalsPerField = new Dictionary<string, double>();

            NumericFieldDefinitions.ToList().ForEach(fd => totalsPerField.Add(fd.Name, 0));

            foreach (WorkItem workItem in workItems)
            {
                foreach (FieldDefinition fieldDefinition in NumericFieldDefinitions)
                {
                    if (!workItem.Fields.Contains(fieldDefinition.Name))
                    {
                        continue;
                    }

                    var fieldValue = workItem.Fields[fieldDefinition.Name].Value;

                    if (fieldValue is double)
                    {
                        totalsPerField[fieldDefinition.Name] += (double)fieldValue;
                    }
                }
            }

            return totalsPerField;
        }

        private static void RemoveZeroValues(Dictionary<string, double> totalsPerField)
        {
            totalsPerField.Where(kv => kv.Value == 0).Select(kv => kv.Key).ToList()
                .ForEach(f => totalsPerField.Remove(f));
        }
    }
}