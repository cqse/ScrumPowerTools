using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using ScrumPowerTools.Services;

namespace ScrumPowerTools.Framework.Extensions
{
    public static class WorkItemStoreExtensions
    {
        public static WorkItemCollection GetWorkItems(this WorkItemStore store, QueryPath queryPath)
        {
            QueryDefinition definition = GetQueryDefinition(store, queryPath);

            return GetWorkItems(store,
                new Query(store, definition.QueryText, new Hashtable {{"project", queryPath.ProjectName}}),
                null);
        }

        public static QueryDefinition GetQueryDefinition(this WorkItemStore store, QueryPath queryPath)
        {
            var queryHierarchy = store.Projects[queryPath.ProjectName].QueryHierarchy;

            var foundQueryItem = queryPath.PathNames
                .Aggregate<string, QueryItem>(queryHierarchy,
                    (queryItem, name) =>
                    {
                        var queryFolder = queryItem as QueryFolder;

                        if (queryFolder != null && queryFolder.Contains(name))
                        {
                            return queryFolder[name];
                        }

                        return null;
                    }
                );

            return foundQueryItem as QueryDefinition;
        }

        public static WorkItemCollection GetWorkItems(this WorkItemStore store, Query workItemQuery,
                                                      IEnumerable<FieldDefinition> fieldDefinitions)
        {
            if (workItemQuery.IsLinkQuery)
            {
                return GetWorkItemsFromLinkQuery(store, workItemQuery, fieldDefinitions);
            }
            else
            {
                return GetWorkItemsFromNormalQuery(workItemQuery, fieldDefinitions);
            }
        }

        private static WorkItemCollection GetWorkItemsFromLinkQuery(WorkItemStore store, Query workItemQuery,
                                                                    IEnumerable<FieldDefinition> fieldDefinitions)
        {
            var workItemInfo = workItemQuery.RunLinkQuery();

            var allWorkItemIds = workItemInfo.Select(wi => wi.TargetId).Distinct().ToArray();

            if (fieldDefinitions == null)
            {
                return store.Query(allWorkItemIds, "SELECT * FROM WorkItems");
            }
            else
            {
                IEnumerable<string> numericFieldDefinitionNames =
                    fieldDefinitions.Select(fd => "[" + fd.ReferenceName + "]");
                string select = "SELECT " + string.Join(", ", numericFieldDefinitionNames) + " FROM WorkItems";

                var actualWorkItemQuery = new Query(store, select, allWorkItemIds);

                return actualWorkItemQuery.RunQuery();
            }
        }

        private static WorkItemCollection GetWorkItemsFromNormalQuery(Query workItemQuery,
                                                                      IEnumerable<FieldDefinition> fieldDefinitions)
        {
            if (fieldDefinitions != null)
            {
                workItemQuery.DisplayFieldList.Clear();

                fieldDefinitions.ToList()
                    .ForEach(fn => workItemQuery.DisplayFieldList.Add(fn));                
            }

            return workItemQuery.RunQuery();
        }
    }
}