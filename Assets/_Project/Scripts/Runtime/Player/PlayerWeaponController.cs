using Unity.Netcode;
using UnityEngine;
using VS.NetcodeExampleProject.Audio;
using VS.Utilities.Input;
using VS.NetcodeExampleProject.Weapons;

namespace VS.NetcodeExampleProject.Player {
    public class PlayerWeaponController : NetworkBehaviour {
        [SerializeField] private InputReader inputReader;
        
        [Header("Shoot References")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;

        public override void OnNetworkSpawn() {
            if (!IsOwner) {
                enabled = false;
            }
        }

        private void Start() {
            if (!IsOwner) {
                return;
            }
            
            inputReader.OnAttackEvent += OnPlayerAttack;
        }

        private void OnPlayerAttack(bool pressed) {
            if (!pressed) {
                return;
            }
            
            PlayerAttackServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        }

        // RPCs executed on the server are executed in a different state context than the client.
        // What this means:
        //      - When executed on the server, class members (such as 'projectileSpawnPoint.position' and 'projectileSpawnPoint.rotation')
        //        can have different values than on the client.
        //      - Data that can be different between the server and client must be sent directly in the RPC. This is why
        //        'projectileSpawnPoint.position' and 'projectileSpawnPoint.rotation' are sent in the RPC. (The 'projectilePrefab'
        //        will always be the same, so that is why it is not sent directly in the RPC.
        [Rpc(SendTo.Server)] 
        private void PlayerAttackServerRpc(Vector3 spawnPosition, Quaternion spawnRotation) {
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
            
            networkObject.Spawn();
            // TODO: Is this the cleanest way?
            // TODO: If using dedicated servers, 'OnEnemyHit' can be called in a different context than the client.
            projectileController.OnEnemyHit += OnEnemyHitRpc;
        }
        

        // THIS WILL NOT WORK
/*
        [Rpc(SendTo.Owner)]
        private void RespondToCallerRpc(NetworkBehaviourReference networkObjectReference) {
            if (networkObjectReference.TryGet(out ProjectileController projectileController)) {
                projectileController.OnEnemyHit += OnEnemyHit;
            }
            
            Debug.LogWarning("This should never be called!");
        }
*/

        [Rpc(SendTo.Owner)]
        private void OnEnemyHitRpc() {
            Debug.Log($"Update Score invoked from {NetworkManager.Singleton.LocalClientId}");
            NetworkedAudioManager.Instance.PlaySoundAtPosition(0, transform.position, 1f);
        }
        
        public override void OnDestroy() {
            base.OnDestroy();
            
            inputReader.OnAttackEvent -= OnPlayerAttack;
        }
    }
}