# Project Overview
- Game Title: Duskroot-Lullaby
- High-Level Concept: Top-down/Action game where the player aims and attacks enemies and breakable objects.
- Players: Single player / Multiplayer (using Fusion).
- Target Platform: Standalone Windows.
- Render Pipeline: Custom/URP (PC_RPAsset).
- Input System: New Input System.

# Game Mechanics
## Core Gameplay Loop
- Players move and aim with the mouse.
- Holding the aim button displays a red dot (reticle) that snaps to the nearest enemy or breakable object in a 90-degree cone.
- Attacking while a target is locked causes the character to face that target for the duration of the attack animation.
- After the attack, the character returns to facing the mouse location.

## Controls and Input Methods
- **Aim (Hold):** Displays the red dot reticle and enables target snapping.
- **Attack (Press):** Triggers the attack and snap-rotation to the target.
- **Mouse:** Controls the base aiming direction and character rotation.

# UI
- **Red Dot Icon:** A visual indicator at the aim point.
- **Visibility:** Only visible when `IsAiming` is true.

# Key Asset & Context
- `Assets/!/Scripts/Gameplay/Player/CharacterLook.cs`: Will be updated to handle reticle logic, target snapping, and attack-rotation overriding.
- `Assets/!/Scripts/Gameplay/CombatSystem/PlayerAttack.cs`: Will be updated to notify `CharacterLook` when an attack starts.
- `Assets/!/Scripts/Objects/Breakable.cs`: Existing component for breakable objects.
- `EnemySetup.cs`: Existing component to identify enemies.

# Implementation Steps

## 1. Prepare the Red Dot Prefab
- Create a simple prefab (e.g., `Assets/!/Prefabs/VFX/AimReticle.prefab`) with a `SpriteRenderer` using a circle sprite and set to a red color.
- Alternatively, provide instructions to the user to assign a prefab to the `CharacterLook` component.

## 2. Modify `CharacterLook.cs`
- **Fields:**
    - `[SerializeField] GameObject reticlePrefab`: To instantiate the red dot.
    - `[SerializeField] float aimConeAngle = 90f`: The snapping cone width.
    - `[SerializeField] float aimRange = 15f`: The snapping range.
    - `[SerializeField] float attackRotationDuration = 0.5f`: How long to stay locked to the target during an attack.
    - `public Transform lockedTarget`: To store the currently snapped object (visible in inspector for debugging).
    - `GameObject reticleInstance`: The local instance of the reticle.
- **Networked State:**
    - `[Networked] Tick attackEndTick { get; set; }`: To track the duration of the rotation lock across the network.
- **Logic Updates:**
    - In `Spawned`, instantiate the `reticleInstance` from `reticlePrefab` (if assigned) and hide it initially.
    - In `FixedUpdateNetwork`:
        - If `data.Aim` is true:
            - Perform a cone cast using `Physics.OverlapSphere` and `Vector3.Angle`.
            - Filter for components `EnemySetup` and `Breakable`.
            - Update `lockedTarget` to the closest valid target.
            - Calculate `lookingAt`: if `lockedTarget` is found, use its position; otherwise, use the mouse-to-ground raycast position.
            - Show and move the reticle instance to `lookingAt`.
        - Handle Rotation:
            - If `Runner.Tick < attackEndTick` and `lockedTarget != null`:
                - Call `RotateTo(lockedTarget.position - transform.position, this)` to face the target.
            - Else if `data.Aim`:
                - Call `RotateTo(mouseGroundPos - transform.position, this)` to face the mouse position.
    - Add a public method `OnAttackTriggered()`:
        - Sets `attackEndTick = Runner.Tick + (int)(attackRotationDuration / Runner.DeltaTime)`.

## 3. Modify `PlayerAttack.cs`
- Update the attack execution block to notify `CharacterLook` of the attack.
- Inside `FixedUpdateNetwork`, when an attack is triggered (`isButtonPressed && !lastAttack`):
    - Call `characterLook.OnAttackTriggered()`.

# Verification & Testing
1. **Reticle Visibility:** Verify that the red dot only appears when the aim button is held.
2. **Snapping Logic:** Verify that the red dot snaps to enemies (`EnemySetup`) or breakable objects (`Breakable`) when they are within the 15-unit range and 90-degree cone.
3. **Attack Rotation:** Verify that attacking while a target is snapped causes the player to immediately face the target.
4. **Post-Attack Reset:** Verify that after the attack rotation duration, the player's rotation returns to following the mouse.
5. **Network Sync:** Confirm that rotation changes on the authority are correctly seen by proxies via `SimpleKCC`.
