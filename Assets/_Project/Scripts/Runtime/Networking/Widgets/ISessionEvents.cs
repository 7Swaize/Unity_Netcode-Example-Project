namespace VS.NetcodeExampleProject.Networking {
    public interface ISessionEvents {
        public void OnPlayerJoinedSession(string playerId) { }
        public void OnPlayerLeftSession(string playerId) { }
    }
}