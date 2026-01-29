using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    /// <summary>
    /// Handles creating, joining, leaving, and managing multiplayer sessions.
    /// Acts as a global singleton and initializes Unity Services required for sessions.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class SessionHandler : Singleton<SessionHandler> {
        [SerializeField] private SessionConfig sessionConfig;

        /// <summary>
        /// The active session instance the player is currently connected to.
        /// </summary>
        public ISession ActiveSession { get; private set; }

        public event Action OnSessionJoining = delegate { };
        public event Action<SessionException> OnSessionFailedToJoin = delegate { };
        public event Action<ISession> OnSessionJoined = delegate { };
        public event Action OnSessionLeft = delegate { };

        private async void Start() {
            IServiceInitialization serviceInitialization = new SessionServiceInitialization();
            await serviceInitialization.InitializeAsync();
        }

        /// <summary>
        /// Creates a new session as the host using the provided configuration.
        /// </summary>
        /// <param name="config">Session creation parameters.</param>
        public async Task CreateSessionAsHostAsync(SessionConfig config) {
            Dictionary<string, PlayerProperty> playerProperties = await GetPlayerProperties();

            SessionOptions sessionOptions = new SessionOptions {
                MaxPlayers = config.maxPlayers,
                IsLocked = config.isLocked,
                IsPrivate = config.isPrivate,
                Name = config.GetUniqueSessionName(),
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
        
        /// <summary>
        /// Attempts to join a session using a human-readable session code.
        /// </summary>
        /// <param name="sessionCode">The session code shared by the host.</param>
        public async Task JoinSessionByCodeAsync(string sessionCode) {
            Dictionary<string, PlayerProperty> playerProperties = await GetPlayerProperties();
            JoinSessionOptions sessionOptions = new JoinSessionOptions {
                PlayerProperties = playerProperties
            };

            await TryJoinOrCreateSessionAsync(async () => {
                ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode, sessionOptions);
            });
        }

        /// <summary>
        /// Attempts to join an existing session using its unique session ID.
        /// </summary>
        /// <param name="sessionId">The ID of the session to join.</param>
        public async Task JoinSessionByIdAsync(string sessionId) {
            Dictionary<string, PlayerProperty> playerProperties = await GetPlayerProperties();
            JoinSessionOptions sessionOptions = new JoinSessionOptions {
                PlayerProperties = playerProperties
            };

            await TryJoinOrCreateSessionAsync(async () => {
                ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId, sessionOptions);
            });
        }

        /// <summary>
        /// Removes a player from the session if the caller is the host.
        /// </summary>
        /// <param name="playerId">The ID of the player to remove.</param>
        public async Task KickPlayerAsync(string playerId) {
            if (!ActiveSession.IsHost) {
                return;
            }

            await ActiveSession.AsHost().RemovePlayerAsync(playerId);
        }

        /// <summary>
        /// Queries available sessions with optional search filters.
        /// </summary>
        /// <param name="options">Filtering options for querying sessions.</param>
        /// <returns>A list of all sessions matching the query.</returns>
        private async Task<IList<ISessionInfo>> QuerySessionsAsync(QuerySessionsOptions options = default) {
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return results.Sessions;
        }

        /// <summary>
        /// Leaves the active session and resets the internal state.
        /// </summary>
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

        // Allows you to define custom player properties (such as name, rank, level, etc.) to be set when joining a session.
        // There is a limit (I think it's like 32 properties) to the number of properties you can set.
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