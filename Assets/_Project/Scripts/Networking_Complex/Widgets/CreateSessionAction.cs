using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class CreateSessionAction : SessionActionBase, ISetupEvents {
        [SerializeField] private SessionConfig sessionConfig;
        [SerializeField] private TMP_InputField sessionNameField;

        public override void OnServicesInitialized() {
            sessionNameField.onEndEdit.AddListener(value => {
                if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(value)) {
                    SessionAction();
                }
            });
            
            sessionNameField.onValueChanged.AddListener(value => {
                sessionActionButton.interactable = !string.IsNullOrEmpty(value);
            });
        }
        
        protected override async void SessionAction() {
            sessionConfig.sessionName = sessionNameField.text;
            await SessionHandler.Instance.CreateSessionAsHostAsync(sessionConfig);
        }
        
        public override void OnSessionJoined(ISession session) {
            base.OnSessionJoined(session);
            
            sessionNameField.text = string.Empty;
        }

        public void OnResetButtonClicked() {
            sessionNameField.text = string.Empty;
        }
    }
}