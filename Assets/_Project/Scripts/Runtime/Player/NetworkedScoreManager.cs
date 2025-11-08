using System;
using Unity.Netcode;

// Inherit from NetworkBehaviour to use the NetworkVariable features.
public class NetworkedScoreManager : NetworkBehaviour {
    // Static singleton instance
    public static NetworkedScoreManager Instance { get; private set; }
    // Event that is fired when the score is updated. This is so that players can update their UI.
    public event Action OnScoreUpdated;
    
    // Network Variables to store the player scores.
    private NetworkVariable<int> _playerOneScore = new NetworkVariable<int>();
    private NetworkVariable<int> _playerTwoScore = new NetworkVariable<int>();
    
    // Initializes the singleton instance.
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    public override void OnNetworkSpawn() {
        if (IsClient) {
            // Fires the event when either score NetworkVariable is updated.
            _playerOneScore.OnValueChanged += (_, _) => OnScoreUpdated?.Invoke();
            _playerTwoScore.OnValueChanged += (_, _) => OnScoreUpdated?.Invoke();
        }
    }
    
    // Entry point method that any client can call to update the score.
    public void UpdateScore(ulong playerId) {
        UpdateScoreRpc(playerId);
    }

    // RPC method sent to server because the highest write permissions for a NetworkVariable is 'Server'.
    [Rpc(SendTo.Server)]
    private void UpdateScoreRpc(ulong playerId) {
        // Increment the score for the player with the given ID.
        if (playerId == 0) {
            _playerOneScore.Value++;
        } else if (playerId == 1) {
            _playerTwoScore.Value++;
        }
    }

    // Returns the current scores.
    // Called by the score UI updaters that are local to each client. 
    public (int, int) GetPlayerScores() {
        return (_playerOneScore.Value, _playerTwoScore.Value);
    }
}