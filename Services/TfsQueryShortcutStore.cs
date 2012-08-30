using System.Linq;

namespace ScrumPowerTools.Services
{
    public class TfsQueryShortcutStore
    {
        private const int MaxShortcuts = 5;
        private readonly QueryPath[] shortcuts = new QueryPath[MaxShortcuts];
        private readonly GeneralOptions options;

        public TfsQueryShortcutStore(GeneralOptions options)
        {
            this.options = options;

            LoadShortcutsFromStorage();
        }

        private void LoadShortcutsFromStorage()
        {
            if (options.TfsQueryShortcuts != null)
            {
                for (int i = 0; i < options.TfsQueryShortcuts.Length && i < MaxShortcuts; i++)
                {
                    if (!string.IsNullOrWhiteSpace(options.TfsQueryShortcuts[i]))
                    {
                        shortcuts[i] = new QueryPath(options.TfsQueryShortcuts[i]);
                    }
                }
            }
        }

        public void Assign(uint shortcutNr, QueryPath queryPath)
        {
            if ((shortcutNr < MaxShortcuts) && (queryPath != null))
            {
                shortcuts[shortcutNr] = queryPath;

                PersistShortcuts();
            }
        }

        private void PersistShortcuts()
        {
            options.TfsQueryShortcuts = 
                (from shortcut in shortcuts
                select shortcut != null ? shortcut.ToString() : "").ToArray();

            options.SaveSettingsToStorage();
        }

        public QueryPath GetShortcut(uint shortcutNr)
        {
            if (shortcutNr < MaxShortcuts)
            {
                return shortcuts[shortcutNr];
            }

            return null;
        }
    }
}