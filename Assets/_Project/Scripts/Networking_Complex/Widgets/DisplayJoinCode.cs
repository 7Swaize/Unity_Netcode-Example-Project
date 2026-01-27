using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VS.NetcodeExampleProject.Networking {
    public class DisplayJoinCode : WidgetBehaviour, ISessionLifecycleEvents {
        private ISession _activeSession;
        
        [SerializeField] private TMP_Text joinCodeText;
        [SerializeField] private Button copyCodeButton;

        private const string k_emptyJoinCode = "-";

        private void Start() {
            copyCodeButton.onClick.AddListener(CopyCodeToClipboard);
        }

        public override void OnServicesInitialized() {
            copyCodeButton.interactable = false;
            joinCodeText.text = k_emptyJoinCode;
        }

        public void OnSessionJoined(ISession session) {
            _activeSession = session;
            joinCodeText.text = _activeSession.Code;
            copyCodeButton.interactable = true;
        }
        
        public void OnSessionLeft() {
            _activeSession = null;
            joinCodeText.text = k_emptyJoinCode;
            copyCodeButton.interactable = false;
        }
        
        private void CopyCodeToClipboard() {
            EventSystem.current.SetSelectedGameObject(null);
            if (_activeSession == null) {
                return;
            }
            
            GUIUtility.systemCopyBuffer = _activeSession.Code;
        }
    }
}