using System;
using Unity.Netcode;
using UnityEngine;
using VS.NetcodeExampleProject.Player;

namespace _Project.Scripts.Runtime.Weapons {
    public class ProjectileController : NetworkBehaviour {
        [Header("Bullet Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float lifetime;
        
        public event Action OnEnemyHit = delegate { };

        private float _lifeTimer;
        private Rigidbody _rigidbody;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }

        private void Init() {
            _lifeTimer = lifetime;
        }

        public override void OnNetworkSpawn() {
            if (!IsServer) {
                enabled = false; 
                return;
            }

            Init();
        }

        private void Update() {
            HandleMovementAndLifetime();
        }

        private void HandleMovementAndLifetime() {
            transform.position += transform.forward * (speed * Time.deltaTime);
            _lifeTimer -= Time.deltaTime;

            if (_lifeTimer <= 0 && IsSpawned) {
                NetworkObject.Despawn();
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!IsServer) {
                return;
            }
            
            if (other.TryGetComponent(out IDamageable _)) {
                OnEnemyHit.Invoke();
            }

            // TODO: See why unwrapping this in 'IsSpawned' causes there to be an error
            if (IsSpawned) {
                NetworkObject.Despawn();
            }
        }

        public override void OnDestroy() {
            base.OnDestroy();
            OnEnemyHit = null;
        }
    }
}