# Unity Netcode Learning Project

Educational project demonstrating Unity Netcode for GameObjects (NGO) and Unity Multiplayer Services. Built as a reference for learning multiplayer game development fundamentals.

UI aspects adapted from Unity Multiplayer Widgets package. 

## What It Does

Two-player shooter where players fire projectiles at each other to score points. Includes session creation/joining via Unity Multiplayer Services (relay or direct connection) with a simple UI for matchmaking.

## Core Netcode Concepts

**NetworkedPlayerController.cs** - Player movement and shooting. Only the owning client processes input (`IsOwner` check). `[Rpc(SendTo.Server)]` sends fire request to server which spawns projectile for all clients. `[Rpc(SendTo.Owner)]` callback updates score when projectile hits opponent.

**NetworkedProjectileController.cs** - Server-authoritative projectile. Movement only runs on server (`IsServer` check). `OnTriggerEnter` only processes on server to prevent duplicate hit detection. Uses `NetworkObject.Spawn()` and `Despawn()`.

**NetworkedScoreManager.cs** - Synced score tracking using `NetworkVariable<int>`. Clients call RPC to update score, server increments the NetworkVariable, OnValueChanged callback fires on all clients to update UI.

**NetworkedAudioManager.cs** - Positional audio sync. Client requests sound via ServerRpc, server broadcasts to all clients via ClientRpc. Spawns temporary AudioSource at position then destroys after clip plays.

## Session Management (Advanced)

**SessionHandler.cs** - Manages Unity Multiplayer Service sessions. Creates/joins sessions with relay or direct networking. Anonymous auth via `AuthenticationService`.

**SessionWidgetEventDispatcher.cs** - Event bus pattern. Widgets register to receive session lifecycle events (joining/joined/left) and player events (joined/left session). Prevents tight coupling between UI components.

**Widget System** - Reusable UI components (`CreateSessionAction`, `JoinSessionActionByCode`, `DisplayJoinCode`, `SessionPlayerList`) that automatically respond to session state changes. Inherit from `WidgetBehaviour` and implement event interfaces.

## Simple vs Complex

Two implementations provided:

**Networking_Simple/** - Bare minimum for creating/joining sessions (~100 lines total). Good starting point.

**Networking_Complex/** - Production-ready architecture with widget pattern, connection type selection (relay/direct), player list, configurable spawn points. Shows best practices for scalable multiplayer UI.

## Key Patterns

- **Server Authority**: Only server spawns/despawns objects and processes physics
- **RPC Types**: `SendTo.Server` for requests, `SendTo.ClientsAndHost` for broadcasts, `SendTo.Owner` for individual client updates
- **NetworkVariables**: Automatic replication when value changes on server
- **IsOwner/IsServer**: Guard clauses prevent duplicate logic execution
- **Connection Approval**: `SessionConnectionApproval.cs` sets spawn position when players join
