using TMPro;
using UnityEngine;
using Unity.Services.Multiplayer;


namespace VS.NetcodeExampleProject.Networking {
    public class LeaveSession : SessionActionBase {
        [SerializeField] private TMP_Text leaveButtonText;

        private const string k_hostLeaveText = "Leave and Terminate Session";
        private const string k_clientLeaveText = "Leave Session";

        protected override async void SessionAction() {
            await SessionHandler.Instance.LeaveSessionAsync();
        }

        public override void OnSessionJoined(ISession session) {
            base.OnSessionJoined(session);

            leaveButtonText.text = session.IsHost ? k_hostLeaveText : k_clientLeaveText;

            sessionActionButton.interactable = true;
            sessionActionButton.onClick.AddListener(SessionAction);
        }
        
        public override void OnSessionLeft() {
            base.OnSessionLeft();
            
            sessionActionButton.interactable = false;
            sessionActionButton.onClick.RemoveAllListeners();
        }
    }
}