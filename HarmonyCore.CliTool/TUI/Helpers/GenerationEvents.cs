using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Helpers
{
    internal class GenerationEvents
    {
        public Action<string, float> ProgressUpdate;
        public Action<string> StatusUpdate;
        public Action<string> Message;
        public Action Loaded;
        public void OnLoaded()
        {
            Loaded?.Invoke();
            _loading.TrySetResult(true);
        }

        private readonly TaskCompletionSource<bool> _loading = new TaskCompletionSource<bool>();
        //This is just a little bit janky for multi part operations watch out for odd behaviors
        public Task LoadedAsync => _loading.Task;
        public CancellationToken CancelToken;

        public GenerationEvents()
        {

        }

        public GenerationEvents(GenerationEvents parent, Action loadedHandler)
        {
            ProgressUpdate = parent.ProgressUpdate;
            StatusUpdate = parent.StatusUpdate;
            Message = parent.Message;
            Loaded = loadedHandler;
        }
    }
}
