using Unity.Services.Multiplayer;

namespace VS.NetcodeExampleProject.Networking {
    public interface ISessionLifecycleEvents {
        public void OnSessionJoining() { }
        public void OnSessionFailedToJoin(SessionException sessionException) { }
        public void OnSessionJoined(ISession session) { }
        public void OnSessionLeft() { }
    }
}