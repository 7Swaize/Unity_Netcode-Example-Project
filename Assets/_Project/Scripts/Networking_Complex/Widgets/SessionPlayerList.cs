using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class SessionPlayerList : WidgetBehaviour, ISessionLifecycleEvents, ISessionEvents {
        private ISession _activeSession;
        
        [Header("References")]
        [SerializeField] private GameObject listItemCopy;
        [SerializeField] private Transform listRoot;
        
        private Dictionary<string, SessionPlayerListItem> _playerListItems = new Dictionary<string, SessionPlayerListItem>();

        protected void OnEnable() {
            UpdatePlayerList();
        }
        
        protected void OnDisable() {
            ClearPlayerList();
        }
        
        public void OnSessionJoined(ISession session) {
            _activeSession = session;
            UpdatePlayerList();
        }

        public void OnSessionLeft() {
            _activeSession = null;
            ClearPlayerList();
        }
        
        public void OnPlayerJoinedSession(string playerId) => UpdatePlayerList();
        public void OnPlayerLeftSession(string playerId) => RemovePlayerFromList(playerId);

        private void ClearPlayerList() {
            foreach (SessionPlayerListItem playerListItem in _playerListItems.Values) {
                Destroy(playerListItem.gameObject);
            }
            
            _playerListItems.Clear();
        }

        private void UpdatePlayerList() {
            if (_activeSession == null) {
                return;
            }

            foreach (IReadOnlyPlayer player in _activeSession.Players) {
                string playerId = player.Id;
                if (_playerListItems.ContainsKey(playerId)) {
                    continue;
                }
                
                SessionPlayerListItem item =  Instantiate(listItemCopy, listRoot).GetComponent<SessionPlayerListItem>();
                _playerListItems.Add(playerId, item);

                string playerName = "Anonymous";
                if (player.Properties.TryGetValue(SessionConstants.k_playerNamePropertyKey,
                        out var playerNameProperty)) {
                    playerName = playerNameProperty.Value;
                }
                
                item.Init(playerName, playerId);
                item.gameObject.SetActive(true);
            }
        }
        
        private void RemovePlayerFromList(string playerId) {
            if (_activeSession == null) {
                return;
            }
            
            if (_playerListItems.TryGetValue(playerId, out var playerListItem)) {
                Destroy(playerListItem.gameObject);
                _playerListItems.Remove(playerId);
            }
        }
    }
}