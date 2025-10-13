using TMPro;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class CreateSession : EnterSessionBase {
        [SerializeField] protected SessionConfig sessionConfig;
        [SerializeField] private TMP_InputField sessionNameField;

        public override void OnServicesInitialized() {
            sessionNameField.onEndEdit.AddListener(value => {
                if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(value)) {
                    EnterSession();
                }
            });
            
            sessionNameField.onValueChanged.AddListener(value => {
                enterSessionButton.interactable = !string.IsNullOrEmpty(value);
            });
        }
        
        protected override async void EnterSession() {
            sessionConfig.sessionName = sessionNameField.text;
            await SessionHandler.Instance.CreateSessionAsHostAsync(sessionConfig);
        }
    }
}