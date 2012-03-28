using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace ScrumPowerTools.Framework.Composition
{
    public static class IoC
    {
        private static CompositionContainer container;

        public static void Setup(Assembly assembly)
        {
            container = new CompositionContainer(new AssemblyCatalog(assembly));
        }

        public static void Register<T>(T instance)
        {
            container.ComposeExportedValue(instance);
        }

        public static T GetInstance<T>()
        {
            return container.GetExportedValue<T>();
        }

        public static IEnumerable<T> GetInstances<T>()
        {
            return container.GetExportedValues<T>();
        }
    }
}