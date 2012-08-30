using System;
using System.Linq;

namespace ScrumPowerTools.Services
{
    public class QueryPath
    {
        public QueryPath(string queryPath)
        {
            var queryPathElements = queryPath.Split('/');

            if (!queryPathElements.Any())
            {
                throw new ArgumentException("The specified path is not valid."); // no elements
            }

            ProjectName = queryPathElements.First().TrimStart('$');

            PathNames = queryPathElements.Skip(1).ToArray();
        }

        public QueryPath(string projectName, string queryPath)
        {
            ProjectName = projectName;
            PathNames = queryPath.Split('/');
        }

        public string ProjectName { get; private set; }
        public string[] PathNames { get; private set; }

        public override string ToString()
        {
            return "$" + ProjectName + "/" + string.Join("/", PathNames);
        }
    }
}