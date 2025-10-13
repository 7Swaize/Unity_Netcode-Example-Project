using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;
using VS.Utilities.Singletons;

namespace VS.NetcodeExampleProject.Networking {
    public class SessionWidgetEventDispatcher : Singleton<SessionWidgetEventDispatcher> {
        private ISession _activeSession;
        
        private readonly List<IWidget> _widgets = new List<IWidget>();
        private readonly List<ISessionLifecycleEvents> _sessionLifecycleListeners = new List<ISessionLifecycleEvents>();
        private readonly List<ISessionEvents> _sessionEventListeners = new List<ISessionEvents>();
        
        public void Start() {
            SessionHandler.Instance.OnSessionJoining += OnSessionJoining;
            SessionHandler.Instance.OnSessionFailedToJoin += OnSessionFailedToJoin;
            SessionHandler.Instance.OnSessionJoined += OnSessionJoined;
            SessionHandler.Instance.OnSessionLeft += OnSessionLeft;
        }
        
        public void OnServicesInitialized() {
            foreach (IWidget widget in _widgets) {
                widget.OnServicesInitialized();
            }
        }

        public void RegisterWidget(IWidget widget) {
            if (widget is ISessionLifecycleEvents sessionLifecycleListener) {
                _sessionLifecycleListeners.Add(sessionLifecycleListener);
            }

            if (widget is ISessionEvents sessionEventListener) {
                _sessionEventListeners.Add(sessionEventListener);
            }
            
            _widgets.Add(widget);
        }

        public void DeregisterWidget(IWidget widget) {
            if (widget is ISessionLifecycleEvents sessionLifecycleListener) {
                _sessionLifecycleListeners.Remove(sessionLifecycleListener);
            }

            if (widget is ISessionEvents sessionEventListener) {
                _sessionEventListeners.Remove(sessionEventListener);
            }
            
            _widgets.Remove(widget);
        }
        
        private void OnSessionJoining() {
            foreach (ISessionLifecycleEvents sessionLifecycleListener in _sessionLifecycleListeners) {
                sessionLifecycleListener.OnSessionJoining();
            }
        }
        
        private void OnSessionFailedToJoin(SessionException exception) {
            foreach (ISessionLifecycleEvents sessionLifecycleListener in _sessionLifecycleListeners) {
                sessionLifecycleListener.OnSessionFailedToJoin(exception);
            }
        }
        
        private void OnSessionJoined(ISession session) {
            session.PlayerJoined += OnPlayerJoinedSession;
            session.PlayerLeaving += OnPlayerLeftSession;
            
            foreach (ISessionLifecycleEvents sessionLifecycleListener in _sessionLifecycleListeners) {
                sessionLifecycleListener.OnSessionJoined(session);
            }
            
            _activeSession = session;
        }
        
        private void OnSessionLeft() {
            _activeSession.PlayerJoined -= OnPlayerJoinedSession;
            _activeSession.PlayerLeaving -= OnPlayerLeftSession;
            
            foreach (ISessionLifecycleEvents sessionLifecycleListener in _sessionLifecycleListeners) {
                sessionLifecycleListener.OnSessionLeft();
            }
            
            _activeSession = null;
        }

        private void OnPlayerJoinedSession(string playerId) {
            foreach (ISessionEvents sessionEventListener in _sessionEventListeners) {
                sessionEventListener.OnPlayerJoinedSession(playerId);
            }
        }
        
        private void OnPlayerLeftSession(string playerId) {
            foreach (ISessionEvents sessionEventListener in _sessionEventListeners) {
                sessionEventListener.OnPlayerLeftSession(playerId);
            }
        }
    }
}