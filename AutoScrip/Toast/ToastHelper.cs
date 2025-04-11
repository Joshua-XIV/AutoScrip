using Dalamud.Game.Gui.Toast;

namespace AutoScrip.Toast
{
    internal class ToastHelper : IDisposable
    {
        private string? lastToast;
        internal ToastHelper()
        {
            Plugin.ToastGui.Toast += this.OnToast;
            Plugin.ToastGui.QuestToast += this.OnQuestToast;
        }

        public void Dispose()
        {
            Plugin.ToastGui.Toast -= this.OnToast;
            Plugin.ToastGui.QuestToast -= this.OnQuestToast;
        }

        private void OnToast(ref Dalamud.Game.Text.SeStringHandling.SeString message, ref ToastOptions options, ref bool isHandled)
        {
            this.GetText(message);
        }

        private void OnQuestToast(ref Dalamud.Game.Text.SeStringHandling.SeString message, ref QuestToastOptions options, ref bool isHandled)
        {
            this.GetText(message);
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
