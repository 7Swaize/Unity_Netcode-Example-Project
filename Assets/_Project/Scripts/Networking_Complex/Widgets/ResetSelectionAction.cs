using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;

namespace VS.NetcodeExampleProject.Networking {
    public class ResetSelectionAction : SessionActionBase {
        [Header("Reset Selection Events")]
        [Tooltip("Event invoked when the 'Reset Selection' button is pressed. Use this instead of 'OnClick'.")]
        public UnityEvent ResetSelection = new();

        public override void OnServicesInitialized() {
            sessionActionButton.onClick.AddListener(InvokeResetSelection);
        }
        
        protected override void SessionAction() {
            SessionWidgetEventDispatcher.Instance.NotifyResetButtonClicked();
        }
        
        public override void OnSessionJoined(ISession session) {
            base.OnSessionJoined(session);
            
            InvokeResetSelection();
            SessionWidgetEventDispatcher.Instance.NotifyResetButtonClicked();
        }
        
        private void InvokeResetSelection() {
            ResetSelection.Invoke();
        }
    }
}