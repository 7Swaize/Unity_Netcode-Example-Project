using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using VS.Utilities.Singletons;

namespace VS.NetcodeExampleProject.Networking {
    [DefaultExecutionOrder(-100)]
    public class SessionHandler : Singleton<SessionHandler> {
        public ISession ActiveSession { get; private set; }

        public event Action OnSessionJoining = delegate { };
        public event Action<SessionException> OnSessionFailedToJoin = delegate { };
        public event Action<ISession> OnSessionJoined = delegate { };
        public event Action OnSessionLeft = delegate { };

        private async void Start() {
            IServiceInitialization serviceInitialization = new SessionServiceInitialization();
            await serviceInitialization.InitializeAsync();
        }

        public async Task CreateSessionAsHostAsync(SessionConfig config) {
            Dictionary<string, PlayerProperty> playerProperties = await GetPlayerProperties(); 

            SessionOptions sessionOptions = new SessionOptions {
                MaxPlayers = config.maxPlayers,
                IsLocked = config.isLocked,
                IsPrivate = config.isPrivate,
                Name = config.name,
                PlayerProperties = playerProperties
            };

            sessionOptions = config.networkConnectionType switch {
                NetworkConnectionType.Direct => sessionOptions.WithDirectNetwork(config.listenIp, config.publishIp, config.port),
                NetworkConnectionType.Relay => sessionOptions.WithRelayNetwork(),
                _ => sessionOptions.WithDirectNetwork(config.listenIp, config.publishIp, config.port)
            };

            await TryJoinOrCreateSessionAsync(async () => {
                ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(sessionOptions);
            });
        }

        public async Task JoinSessionByIdAsync(string sessionId) {
            OnSessionJoining.Invoke();
            Dictionary<string, PlayerProperty> playerProperties = await GetPlayerProperties();
            JoinSessionOptions sessionOptions = new JoinSessionOptions {
                PlayerProperties = playerProperties
            };
            
            await TryJoinOrCreateSessionAsync(async () => {
                ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId, sessionOptions);
            });
        }

        public async Task JoinSessionByCodeAsync(string sessionCode) {
            OnSessionJoining.Invoke();
            Dictionary<string, PlayerProperty> playerProperties = await GetPlayerProperties();
            JoinSessionOptions sessionOptions = new JoinSessionOptions {
                PlayerProperties = playerProperties
            };
            
            await TryJoinOrCreateSessionAsync(async () => {
                ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode, sessionOptions);
            });
        }

        public async Task KickPlayerAsync(string playerId) {
            if (!ActiveSession.IsHost) {
                return;
            }

            await ActiveSession.AsHost().RemovePlayerAsync(playerId);
        }

        private async Task<IList<ISessionInfo>> QuerySessionsAsync(QuerySessionsOptions options = default) {
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return results.Sessions;
        }

        public async Task LeaveSessionAsync() {
            if (ActiveSession == null) {
                return;
            }

            try {
                await ActiveSession.LeaveAsync();
            }
            catch (SessionException) { }
            finally {
                ActiveSession = null;
                OnSessionLeft.Invoke();
            }
        }
        
        private static async Task<Dictionary<string, PlayerProperty>> GetPlayerProperties() {
            string playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            PlayerProperty playerNameProperty = new PlayerProperty(
                playerName,
                VisibilityPropertyOptions.Public
            );

            return new Dictionary<string, PlayerProperty> {
                { SessionConstants.k_playerNamePropertyKey, playerNameProperty }
            };
        }

        private async Task TryJoinOrCreateSessionAsync(Func<Task> sessionAction) {
            OnSessionJoining.Invoke();

            try {
                await sessionAction();
                OnSessionJoined.Invoke(ActiveSession);
            }
            catch (SessionException ex) {
                OnSessionFailedToJoin.Invoke(ex);
                Debug.LogException(ex);
            }
        }
    }
}