using Unity.Netcode;
using UnityEngine;

// All classes that utilize netcode features must inherit from NetworkBehaviour
public class NetworkedPlayerController : NetworkBehaviour {
    // Movement settings
    public float moveSpeed;
    
    // Camera Settings
    public float mouseSensitivity;
    public Transform playerCamera;
    
    // Combat Settings
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    
    private Rigidbody _rigidbody;
    private float _xRotation;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // On Network Spawn is called when the object is spawned on the server. This is where we can do all netcode related initialization
    // Here we disable the object if we are not the owner of the object.
    // This is done to make sure that only the owning client can control and update its own player instance,
    // while other players remain visible but are not actively processing input or movement logic.
    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            enabled = false;
        }
    }

    // Calls methods for movement.
    private void FixedUpdate() {
        HandleMovement();
    }
    
    // Calls methods for handling firing and camera rotation.
    private void Update() {
        HandleCameraRotation();
        HandleFiring();
    }

    
    private void HandleMovement() {
        // Normal input handling and player transform modifications. Nothing netcode specific here... 
        _rigidbody.linearVelocity = Vector3.zero;

        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(playerCamera.right, Vector3.up).normalized;
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDir += forward;

        if (Input.GetKey(KeyCode.S))
            moveDir -= forward;

        if (Input.GetKey(KeyCode.A))
            moveDir -= right;

        if (Input.GetKey(KeyCode.D))
            moveDir += right;

        if (moveDir.sqrMagnitude > 0f)
            _rigidbody.linearVelocity = moveDir.normalized * moveSpeed;
    }

    private void HandleCameraRotation() {
        // Normal input handling and camera transform modifications. Nothing netcode specific here...
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        if (playerCamera != null) {
            playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }

    // This method is called every frame and checks if the player wants to fire a projectile.
    // If so, it calls the RPC to spawn the projectile on the server.
    private void HandleFiring() {
        if (Input.GetKeyDown(KeyCode.F)) {
            // It's important to send the spawn position and rotation directly in the RPC.
            // RPCs are executed on the server, and the server might not have
            // the same values for 'projectileSpawnPoint.position' and 'projectileSpawnPoint.rotation'.
            PlayerAttackServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        }
    }

    // We define the RPC here.
    // The [Rpc(SendTo.Server)] server attribute makes sure that this method is only executed on the server.
    // All RPC methods must end with the 'Rpc' suffix.
    // We are using a ServerRpc here because only the server has the authority to spawn the projectile.
    // A client cannot directly spawn a projectile.
    [Rpc(SendTo.Server)]
    private void PlayerAttackServerRpc(Vector3 spawnPosition, Quaternion spawnRotation) {
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        NetworkedProjectileController networkedProjectileController = projectile.GetComponent<NetworkedProjectileController>();
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        
        // Spawns the projectile on the network so all clients can see and interact with it.
        networkObject.Spawn();
        
        // 'OnEnemyHitRpc' is called when the projectile hits an opponent.
        // This is the easiest and cleanest way to handle things like updating score when the projectile hits and opponent.
        networkedProjectileController.OnEnemyHit += OnEnemyHitRpc;
    }

    // The [Rpc(SendTo.Owner)] attribute makes sure that this method is only executed on the client that owns this NetworkObject.
    // So when the OnEnemyHitRpc() is triggered server-side, Netcode routes that call back to the player who owns this 'NetworkedPlayerController'.
    [Rpc(SendTo.Owner)]
    private void OnEnemyHitRpc() {
        NetworkedScoreManager.Instance.UpdateScore(NetworkManager.Singleton.LocalClientId);
        NetworkedAudioManager.Instance.PlaySoundAtPosition(0, transform.position, 1f);
    }
}
