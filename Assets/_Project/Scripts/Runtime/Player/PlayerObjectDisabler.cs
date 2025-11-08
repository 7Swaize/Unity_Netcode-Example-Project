using Unity.Netcode;
using UnityEngine;


public class PlayerObjectDisabler : NetworkBehaviour {
    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            // Disables all other client instances of these components on each local machine.
            GetComponent<AudioListener>().enabled = false;
            GetComponent<Camera>().enabled = false;
            enabled = false;
        }
    }
}
