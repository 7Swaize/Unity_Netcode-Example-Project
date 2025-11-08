using System;
using Unity.Netcode;
using UnityEngine;

// Inherit from NetworkBehaviour to use the built-in network features.
public class NetworkedProjectileController : NetworkBehaviour {
    // Projectile settings
    public float speed;
    public float lifetime;
    
    // Event that is fired when the projectile hits an enemy.
    public event Action OnEnemyHit = delegate { };

    private float _lifeTimer;
    private Rigidbody _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
    }

    // Done to make sure that all projectile updates are done on the server.
    public override void OnNetworkSpawn() {
        if (!IsServer) {
            enabled = false; 
            return;
        }

        _lifeTimer = lifetime;
    }
    
    private void Update() {
        HandleMovementAndLifetime();
    }

    // Handles movement and despawns the projectile if it reaches its lifetime.
    private void HandleMovementAndLifetime() {
        transform.position += transform.forward * (speed * Time.deltaTime);
        _lifeTimer -= Time.deltaTime;

        if (_lifeTimer <= 0 && IsSpawned) {
            NetworkObject.Despawn(); // Despawn the projectile for all clients. 
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Unity’s physics system calls OnTriggerEnter(Collider other) on all client-side clones.
        // That's why we need this guard clause.
        if (!IsServer) {
            return;
        }
        
        // Invoke 'OnEnemyHit' if the projectile hits a player.
        if (other.CompareTag("Player")) {
            OnEnemyHit.Invoke();
        }

        if (IsSpawned) {
            // Despawn the projectile for all clients.
            NetworkObject.Despawn();
        }
    }

    // Remove all event subscribers after projectile is destroyed.
    public override void OnDestroy() {
        base.OnDestroy();
        OnEnemyHit = null;
    }
}
