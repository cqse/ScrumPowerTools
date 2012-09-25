using ScrumPowerTools.TfsIntegration;

namespace ScrumPowerTools.Services
{
    public class TfsQueryShortcutAssigner
    {
        private readonly TfsQueryShortcutStore shortcutStore;
        private readonly IVisualStudioAdapter visualStudioAdapter;

        public TfsQueryShortcutAssigner(TfsQueryShortcutStore shortcutStore, IVisualStudioAdapter visualStudioAdapter)
        {
            this.shortcutStore = shortcutStore;
            this.visualStudioAdapter = visualStudioAdapter;
        }

        public void Assign(uint shortcutNr)
        {
            shortcutNr = shortcutNr & 0x0f;

            QueryPath queryPath = visualStudioAdapter.GetCurrentSelectedQueryPath();

            if (queryPath != null)
            {
                shortcutStore.Assign(shortcutNr, queryPath);
            }
        }
    }
}