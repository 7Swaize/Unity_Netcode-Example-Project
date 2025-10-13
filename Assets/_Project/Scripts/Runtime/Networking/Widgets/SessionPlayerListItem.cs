using TMPro;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class SessionPlayerListItem : WidgetBehaviour {
        [Header("References")]
        [SerializeField] private TMP_Text playerNameText;

        private string _playerId;
        private string _playerName;
        
        public void Init(string playerName, string playerId) {
            _playerId = playerId;
            _playerName = playerName;

            if (playerNameText != null)
                playerNameText.text = playerName;
        }
    }
}