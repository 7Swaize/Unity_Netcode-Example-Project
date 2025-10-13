using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VS.NetcodeExampleProject.Networking {
    public abstract class WidgetBehaviour : MonoBehaviour, IWidget {
        protected virtual void OnEnable() {
            SessionWidgetEventDispatcher.Instance.RegisterWidget(this);
        }

        protected virtual void OnDisable() {
            SessionWidgetEventDispatcher.Instance.DeregisterWidget(this);
        }
        
        public virtual void OnServicesInitialized() { }
    }
    
    public abstract class EnterSessionBase : WidgetBehaviour, ISessionLifecycleEvents {
        [SerializeField] protected Button enterSessionButton;
        
        [Header("Join Session Events")]
        [Tooltip("Event invoked when the user is attempting to join a session.")]
        public UnityEvent JoiningSession = new();
        [Tooltip("Event invoked when the user has successfully joined a session.")]
        public UnityEvent<ISession> JoinedSession = new();
        [Tooltip("Event invoked when the user has failed to join a session.")]
        public UnityEvent<SessionException> FailedToJoinSession = new();
        
        protected abstract void EnterSession();

        protected virtual void Awake() {
            enterSessionButton ??= GetComponentInChildren<Button>();
            enterSessionButton.onClick.AddListener(EnterSession);
            enterSessionButton.interactable = false;
        }
        
        public void OnSessionJoining() {
            JoiningSession.Invoke();
        }
        
        public void OnSessionJoined(ISession session) {
            JoinedSession.Invoke(session);
        }
        
        public void OnSessionFailedToJoin(SessionException exception) {
            FailedToJoinSession.Invoke(exception);
        }
        
        public override void OnServicesInitialized() {
            enterSessionButton.interactable = true;
        }
    }
}