using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class CreateSessionAction : SessionActionBase {
        [SerializeField] private SessionConfig sessionConfig;
        
        protected override async void SessionAction() {
            await SessionHandler.Instance.CreateSessionAsHostAsync(sessionConfig);
        }
    }
}