using TMPro;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class JoinSessionByCode : EnterSessionBase {
        [SerializeField] private TMP_InputField sessionJoinCodeField;
        
        private string _sessionJoinCode;

        public override void OnServicesInitialized() {
            sessionJoinCodeField.onEndEdit.AddListener(value => {
                if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(value)) {
                    EnterSession();
                }
            });
            
            sessionJoinCodeField.onValueChanged.AddListener(value => {
                enterSessionButton.interactable = !string.IsNullOrEmpty(value);
            });
        }

        protected override async void EnterSession() {
            _sessionJoinCode = sessionJoinCodeField.text;
            await SessionHandler.Instance.JoinSessionByCodeAsync(_sessionJoinCode);
        }
    }
}