using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ComponentModel.Composition;

namespace ScrumPowerTools.Framework.Composition
{
    public static class IoC
    {
        private static readonly CompositionContainer container;

        static IoC()
        {
            container = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
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