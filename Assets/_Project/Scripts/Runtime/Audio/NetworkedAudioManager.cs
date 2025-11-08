using Unity.Netcode;
using UnityEngine;


public class NetworkedAudioManager : NetworkBehaviour {
    // Static singleton instance
    public static NetworkedAudioManager Instance { get; private set; }
    
    // Audio settings
    public GameObject audioSourcePrefab; // Prefab to instantiate for each audio source
    public AudioClip[] audioClips; // Array of audio clips to play

    private const float k_bufferTimeAfterClipPlay = 0.1f;
    
    // Create the singleton. Nothing netcode specific here.
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    // If called on a client, it sends a request to the server; if called on the server, it immediately broadcasts to all clients.
    public void PlaySoundAtPosition(int clipIndex, Vector3 position, float volume = 1f) {
        if (!IsServer) {
            // Client sends a request to the server to play the sound
            PlaySoundAtPositionServerRpc(clipIndex, position, volume);
            return;
        }

        // Server directly broadcasts the sound to all clients
        PlaySoundAtPositionClientRpc(clipIndex, position, volume);
    }

    // Executes on the server.
    // Server RPC: called by clients to request the server to play a sound.
    // The server then calls the client RPC to actually play the sound on all clients.
    [Rpc(SendTo.Server)]
    private void PlaySoundAtPositionServerRpc(int clipIndex, Vector3 position, float volume) {
        PlaySoundAtPositionClientRpc(clipIndex, position, volume);
    }

    // Executes on all clients and the host.
    // Client RPC: actually spawns the audio source and plays the clip on all clients and the host.
    // Makes sure all players hear the same sound at the same position.
    [Rpc(SendTo.ClientsAndHost)]
    private void PlaySoundAtPositionClientRpc(int clipIndex, Vector3 position, float volume) {
        if (clipIndex < 0 || clipIndex >= audioClips.Length) {
            Debug.LogWarning($"Invalid audio clip index: {clipIndex}");
            return;
        }

        AudioSource audioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity).GetComponent<AudioSource>();
        AudioClip clipToPlay = audioClips[clipIndex];

        audioSource.clip = clipToPlay;
        audioSource.volume = volume;
        audioSource.Play();

        // Destroy the audio source after the clip finishes playing plus a small buffer time
        Destroy(audioSource.gameObject, clipToPlay.length + k_bufferTimeAfterClipPlay);
    }
}
