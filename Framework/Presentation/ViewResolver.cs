using System;
using System.Windows;
using ScrumPowerTools.Framework.Composition;

namespace ScrumPowerTools.Framework.Presentation
{
    public class ViewResolver
    {
        public static UIElement Resolve<T>()
        {
            var viewTypeName = typeof(T).AssemblyQualifiedName.Replace("Model", string.Empty);
            var viewType = Type.GetType(viewTypeName);
            var view = (UIElement)Activator.CreateInstance(viewType);

            var viewModel = IoC.GetInstance<T>();

            ViewModelBinder.Bind(viewModel, view);

            return view;
        }
    }
}