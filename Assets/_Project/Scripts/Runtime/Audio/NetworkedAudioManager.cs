using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace VS.NetcodeExampleProject.Audio {
    // In your actual game, you should probably use a more robust system that would include:
    //      - An object pool instead of direct AudioSource object instantiation
    //      - A robust system that doesn't direct indexing into an array of audio clips (this is unmaintainable as the project scales)
    //      - A struct or scriptable object parameter that provides configurations on how the audio should be played
    //      - More methods/implementations on how audio can be played. 
    public class NetworkedAudioManager : NetworkBehaviour {
        public static NetworkedAudioManager Instance { get; private set; }

        [SerializeField] private GameObject audioSourcePrefab;
        [SerializeField] private AudioClip[] audioClips;

        private const float k_bufferTimeAfterClipPlay = 0.1f;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void PlaySoundAtPosition(int clipIndex, Vector3 position, float volume = 1f) {
            if (!IsServer) {
                PlaySoundAtPositionServerRpc(clipIndex, position, volume);
                return;
            }

            PlaySoundAtPositionClientRpc(clipIndex, position, volume);
        }

        [Rpc(SendTo.Server)]
        private void PlaySoundAtPositionServerRpc(int clipIndex, Vector3 position, float volume) {
            PlaySoundAtPositionClientRpc(clipIndex, position, volume);
        }

        [Rpc(SendTo.NotServer)]
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

            Destroy(audioSource.gameObject, clipToPlay.length + k_bufferTimeAfterClipPlay);
        }

        public AudioClip GetAudioClip(int clipIndex) {
            if (clipIndex < 0 || clipIndex >= audioClips.Length) {
                return null;
            }
            
            return audioClips[clipIndex];
        }
    }

    public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField, HideInInspector] private List<TKey> keyData = new List<TKey>();

        [SerializeField, HideInInspector] private List<TValue> valueData = new List<TValue>();

        public void OnBeforeSerialize() {
            keyData.Clear();
            valueData.Clear();

            foreach (var kvp in this) {
                keyData.Add(kvp.Key);
                valueData.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize() {
            Clear();

            for (int i = 0; i < keyData.Count && i < valueData.Count; i++) {
                this[keyData[i]] = valueData[i];
            }
        }
    }
}