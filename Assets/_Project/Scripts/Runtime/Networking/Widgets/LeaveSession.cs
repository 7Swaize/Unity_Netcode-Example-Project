using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace VS.NetcodeExampleProject.Networking {
    public class LeaveSession : WidgetBehaviour, ISessionLifecycleEvents {
        [SerializeField] private Button leaveSessionButton;
        
        public void OnSessionJoined(ISession session) {
            leaveSessionButton.onClick.AddListener(async () => {
                leaveSessionButton.interactable = false;
                try {
                    await SessionHandler.Instance.LeaveSessionAsync();
                } finally {
                    leaveSessionButton.interactable = true;
                }
            });
        }
        
        public void OnSessionLeft() {
            leaveSessionButton.interactable = false;
            leaveSessionButton.onClick.RemoveAllListeners();
        }
    }
}