using Unity.Services.Multiplayer;
using UnityEngine;


namespace VS.NetcodeExampleProject.Networking {
    public class LeaveSession : SessionActionBase {
        protected override async void SessionAction() {
            await SessionHandler.Instance.LeaveSessionAsync();
        }

        public override void OnSessionJoined(ISession session) {
            base.OnSessionJoined(session);

            Debug.Log($"Joined session {session.Id}");
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