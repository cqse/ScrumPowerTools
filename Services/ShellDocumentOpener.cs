using System;
using Microsoft.VisualStudio.Shell;

namespace ScrumPowerTools.Services
{
    public class ShellDocumentOpener
    {
        private readonly IServiceProvider serviceProvider;

        public ShellDocumentOpener(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Open(string path)
        {
            VsShellUtilities.OpenDocument(serviceProvider, path);
        }
    }
}