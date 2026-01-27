using System;
using System.Collections.Generic;
using Unity.Services.Multiplayer;

namespace VS.NetcodeExampleProject.Networking {
    public sealed class SessionWidgetEventDispatcher : Singleton<SessionWidgetEventDispatcher> {
        private sealed class InterfaceRegistry {
            private readonly Dictionary<Type, object> _map = new();

            public void Register<T>(T instance) where T : class {
                Type type = typeof(T);

                if (!_map.TryGetValue(type, out object bucket)) {
                    bucket = new List<T>();
                    _map.Add(type, bucket);
                }

                ((List<T>)bucket).Add(instance);
            }

            public void Deregister<T>(T instance) where T : class {
                if (_map.TryGetValue(typeof(T), out object bucket)) {
                    ((List<T>)bucket).Remove(instance);
                }
            }

            public List<T> Get<T>() where T : class {
                if (_map.TryGetValue(typeof(T), out object bucket)) {
                    return (List<T>)bucket;
                }

                return null;
            }
        }
        
        private ISession _activeSession;

        private readonly List<IWidget> _widgets = new();
        private readonly InterfaceRegistry _registry = new();

        private bool _servicesInitialized;

        private void Start() {
            SessionHandler.Instance.OnSessionJoining += OnSessionJoining;
            SessionHandler.Instance.OnSessionFailedToJoin += OnSessionFailedToJoin;
            SessionHandler.Instance.OnSessionJoined += OnSessionJoined;
            SessionHandler.Instance.OnSessionLeft += OnSessionLeft;
        }

        public void OnServicesInitialized() {
            _servicesInitialized = true;

            foreach (IWidget widget in _widgets) {
                widget.OnServicesInitialized();
            }
        }

        public void RegisterWidget(IWidget widget) {
            _widgets.Add(widget);

            TryRegister<ISessionLifecycleEvents>(widget);
            TryRegister<ISessionEvents>(widget);
            TryRegister<ISetupEvents>(widget);

            if (_servicesInitialized) {
                widget.OnServicesInitialized();
            }
        }

        public void DeregisterWidget(IWidget widget) {
            _widgets.Remove(widget);

            TryDeregister<ISessionLifecycleEvents>(widget);
            TryDeregister<ISessionEvents>(widget);
            TryDeregister<ISetupEvents>(widget);
        }
        
        private void TryRegister<T>(IWidget widget) where T : class {
            if (widget is T t) {
                _registry.Register(t);
            }
        }
        
        private void TryDeregister<T>(IWidget widget) where T : class {
            if (widget is T t) {
                _registry.Deregister(t);
            }
        }
        
        public void NotifyResetButtonClicked() {
            List<ISetupEvents> listeners = _registry.Get<ISetupEvents>();
            if (listeners == null) return;

            foreach (ISetupEvents listener in listeners) {
                listener.OnResetButtonClicked();
            }
        }

        private void OnSessionJoining() {
            List<ISessionLifecycleEvents> listeners = _registry.Get<ISessionLifecycleEvents>();
            if (listeners == null) return;

            foreach (ISessionLifecycleEvents listener in listeners) {
                listener.OnSessionJoining();
            }
        }

        private void OnSessionFailedToJoin(SessionException exception) {
            List<ISessionLifecycleEvents> listeners = _registry.Get<ISessionLifecycleEvents>();
            if (listeners == null) return;

            foreach (ISessionLifecycleEvents listener in listeners) {
                listener.OnSessionFailedToJoin(exception);
            }
        }

        private void OnSessionJoined(ISession session) {
            _activeSession = session;

            session.PlayerJoined += OnPlayerJoinedSession;
            session.PlayerLeaving += OnPlayerLeftSession;

            List<ISessionLifecycleEvents> listeners = _registry.Get<ISessionLifecycleEvents>();
            if (listeners == null) return;

            foreach (ISessionLifecycleEvents listener in listeners) {
                listener.OnSessionJoined(session);
            }
        }

        private void OnSessionLeft() {
            if (_activeSession != null) {
                _activeSession.PlayerJoined -= OnPlayerJoinedSession;
                _activeSession.PlayerLeaving -= OnPlayerLeftSession;
                _activeSession = null;
            }

            List<ISessionLifecycleEvents> listeners = _registry.Get<ISessionLifecycleEvents>();
            if (listeners == null) return;

            foreach (ISessionLifecycleEvents listener in listeners) {
                listener.OnSessionLeft();
            }
        }

        private void OnPlayerJoinedSession(string playerId) {
            List<ISessionEvents> listeners = _registry.Get<ISessionEvents>();
            if (listeners == null) return;

            foreach (ISessionEvents listener in listeners) {
                listener.OnPlayerJoinedSession(playerId);
            }
        }

        private void OnPlayerLeftSession(string playerId) {
            List<ISessionEvents> listeners = _registry.Get<ISessionEvents>();
            if (listeners == null) return;

            foreach (ISessionEvents listener in listeners) {
                listener.OnPlayerLeftSession(playerId);
            }
        }
    }
}
