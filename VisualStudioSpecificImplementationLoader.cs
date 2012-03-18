using System;
using System.Linq;
using System.Reflection;
using ScrumPowerTools.Interfaces;

namespace ScrumPowerTools
{
    public class VisualStudioSpecificImplementationLoader
    {
        public void RegisterTypes(string version)
        {
            int vsVersion = Convert.ToInt32(Convert.ToDouble(version));
            string vsSpecificAssemblyName = string.Format("ScrumPowerTools.VS{0}", vsVersion);

            var assembly = Assembly.Load(vsSpecificAssemblyName);
            Type typeRegistratorType = assembly.GetTypes().Single(t => t.GetInterfaces().Any(i => i == typeof(ITypeRegistrator)));
            
            var registrator = (ITypeRegistrator)Activator.CreateInstance(typeRegistratorType);
            registrator.Register();
        }
    }
}