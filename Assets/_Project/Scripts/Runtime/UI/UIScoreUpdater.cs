using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIScoreUpdater : MonoBehaviour {
    public TMP_Text playerOneScoreText;
    public TMP_Text playerTwoScoreText;
    
    private void Start() {
        // Subscribe to event to be informed when a player’s score is updated.
        NetworkedScoreManager.Instance.OnScoreUpdated += UpdateScoreUI;
        
        // Directly subscribe to the 'OnClientStarted' on the NetworkManager singleton to hide and show the canvas.
        // This is done to only display the scores when the client actually connects to the server.
        NetworkManager.Singleton.OnClientStarted += () => gameObject?.SetActive(true);
        NetworkManager.Singleton.OnClientStopped += _ => gameObject?.SetActive(false);
        
        // Hide the canvas by default.
        gameObject.SetActive(false);
    }

    // Update the score UI.
    private void UpdateScoreUI() {
        (int, int) scores = NetworkedScoreManager.Instance.GetPlayerScores();

        playerOneScoreText.text = "Player One Score: " + scores.Item1;
        playerTwoScoreText.text = "Player Two Score: " + scores.Item2; 
    }
}