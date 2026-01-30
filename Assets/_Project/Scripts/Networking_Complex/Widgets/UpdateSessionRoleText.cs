using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class UpdateSessionRoleText : WidgetBehaviour, ISessionLifecycleEvents {
        [SerializeField] private TMP_Text sessionRoleText;

        public void OnSessionJoined(ISession session) {
            sessionRoleText.text = session.IsHost ? "Host" : "Client";
        }
    }
}