using System;
using System.Windows.Threading;

namespace ScrumPowerTools.Framework
{
    public static class Executor
    {
        private static Action<Action> executor = action => action();

        static Executor()
        {
            InitializeWithDispatcher();
        }

        private static void InitializeWithDispatcher()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            executor = action =>
            {
                if (dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    dispatcher.BeginInvoke(action);
                }
            };
        }

        public static void ExecuteOnUIThread(this Action action)
        {
            executor(action);
        }
    }
}