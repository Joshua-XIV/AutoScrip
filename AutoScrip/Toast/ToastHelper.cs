using Dalamud.Game.Gui.Toast;

namespace AutoScrip.Toast
{
    internal class ToastHelper : IDisposable
    {
        private string? lastToast;
        internal ToastHelper()
        {
            Plugin.ToastGui.Toast += this.OnToast;
        }

        public void Dispose()
        {
            Plugin.ToastGui.Toast -= this.OnToast;
        }

        private void OnToast(ref Dalamud.Game.Text.SeStringHandling.SeString message, ref ToastOptions options, ref bool isHandled)
        {
            GetText(message);
        }

        private void GetText(Dalamud.Game.Text.SeStringHandling.SeString message)
        {
            lastToast = message.TextValue;
        }

        public string? GetLastToast()
        {
            return lastToast;
        }
    }
}
