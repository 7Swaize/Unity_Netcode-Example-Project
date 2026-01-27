using System;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VS.NetcodeExampleProject.Networking {
    public abstract class WidgetBehaviour : MonoBehaviour, IWidget {
        public virtual void OnServicesInitialized() { }
    }
    
    public abstract class SessionActionBase : WidgetBehaviour, ISessionLifecycleEvents {
        [SerializeField] protected Button sessionActionButton;
        
        [Header("Join Session Events")]
        [Tooltip("Event invoked when the user is attempting to join a session.")]
        public UnityEvent JoiningSession = new();
        [Tooltip("Event invoked when the user has successfully joined a session.")]
        public UnityEvent<ISession> JoinedSession = new();
        [Tooltip("Event invoked when the user has failed to join a session.")]
        public UnityEvent<SessionException> FailedToJoinSession = new();
        [Tooltip("Event is invoked when the user leaves the current session")]
        public UnityEvent LeftSession = new();
        
        protected abstract void SessionAction();

        protected virtual void Awake() {
            sessionActionButton ??= GetComponentInChildren<Button>();
            sessionActionButton.onClick.AddListener(SessionAction);
        }
        
        public override void OnServicesInitialized() {
            sessionActionButton.interactable = true;
        }
        
        public virtual void OnSessionJoining() {
            JoiningSession.Invoke();
        }
        
        public virtual void OnSessionJoined(ISession session) {
            JoinedSession.Invoke(session);
        }
        
        public virtual void OnSessionFailedToJoin(SessionException exception) {
            FailedToJoinSession.Invoke(exception);
        }
        
        public virtual void OnSessionLeft() {
            LeftSession.Invoke();
        }
    }
}