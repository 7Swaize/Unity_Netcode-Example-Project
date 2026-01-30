using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class UpdateConnectionLoadingText : WidgetBehaviour, ISessionLifecycleEvents, ISetupEvents {
        [SerializeField] private TMP_Text connectionLoadingText;
        [SerializeField] private int delayPerTick;
        
        private CancellationTokenSource _connectingCts;

        public void OnSessionJoining() {
            _connectingCts = new CancellationTokenSource();
            _ = AnimateConnectionLoadingTextAsync(_connectingCts.Token);
        }

        public void OnSessionJoined(ISession session) => ClearConnectionLoadingText();
        public void OnResetButtonClicked() => ClearConnectionLoadingText();

        private async Task AnimateConnectionLoadingTextAsync(CancellationToken cts) {
            string baseText = "Connecting";
            int dotCount = 0;

            try {
                while (!_connectingCts.IsCancellationRequested) {
                    connectionLoadingText.text = $"{baseText}{new string('.', dotCount++)}";
                    dotCount %= 4;
                    await Task.Delay(delayPerTick, cts);
                }
            } catch (TaskCanceledException) { }
        }
        
        private void ClearConnectionLoadingText() {
            _connectingCts?.Cancel();
            _connectingCts = null;
            connectionLoadingText.text = string.Empty;
        }
    }
}