using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngine.InputSystem;
using CombatSystem;


/// <summary>
/// Handles player movement including walking and running.
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeedPercentage = 0.3f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float deceleration = 2f;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeedMultiplier = 3f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 2f;

    SimpleKCC characterController;
    Animator animator;
    CharacterLook rotateCharacter;
    PlayerHealth playerHealth;

    float maxSpeed = 5f;
    Vector3 moveDirection;
    Vector3 velocity;

    [Networked] Vector3 NetworkVelocity { get; set; }
    [Networked] TickTimer dashTimer { get; set; }
    [Networked] TickTimer dashCooldownTimer { get; set; }
    [Networked] Vector3 dashDirection { get; set; }

    bool isWalking = false;

    // -- Initialization --

    private void Start()
    {
        characterController = GetComponent<SimpleKCC>();
        animator = GetComponentInChildren<Animator>();
        rotateCharacter = GetComponent<CharacterLook>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public override void Spawned()
    {
        CharacterDataManager.Instance.OnLevelUp += UpdateSpeed;
        UpdateSpeed();
    }

    // -- Simulation --

    public override void FixedUpdateNetwork()
    {
        // Only the State Authority (owner) should handle movement
        if (!HasStateAuthority)
            return;

        // Ensure components are initialized
        if (characterController == null || animator == null || rotateCharacter == null)
            return;

        bool dashInput = false;

        // Get input data from the network
        if (GetInput(out NetworkInputData data))
        {
            moveDirection = new Vector3(data.Move.x, 0, data.Move.y);
            isWalking = data.Walk;
            dashInput = data.Dash;
        }

        // Start dash if input is received and we're not already dashing or on cooldown
        if (dashInput && dashTimer.ExpiredOrNotRunning(Runner) && dashCooldownTimer.ExpiredOrNotRunning(Runner))
        {
            dashTimer = TickTimer.CreateFromSeconds(Runner, dashDuration);
            dashCooldownTimer = TickTimer.CreateFromSeconds(Runner, dashDuration + dashCooldown);
            dashDirection = transform.forward;
        }

        if (!dashTimer.ExpiredOrNotRunning(Runner))
        {
            // Dashing logic
            if (playerHealth != null) playerHealth.IsInvulnerable = true;

            velocity = dashDirection * (maxSpeed * dashSpeedMultiplier);
            characterController.Move(velocity);
        }
        else
        {
            // Normal movement logic
            if (playerHealth != null) playerHealth.IsInvulnerable = false;

            if (rotateCharacter.IsAiming())
                isWalking = true;

            // Determine the speed limit based on whether the player is walking
            float speedLimit = isWalking ? maxSpeed * walkSpeedPercentage : maxSpeed;

            // Calculate movement direction based on input and move the character
            velocity = Vector3.MoveTowards(velocity, moveDirection * speedLimit, (moveDirection.magnitude > 0 ? acceleration : deceleration));
            characterController.Move(velocity);

            // Rotate player to face movement direction
            if (moveDirection != Vector3.zero)
            {
                rotateCharacter.RotateTo(moveDirection, this);
            }
        }

        // Update networked properties
        NetworkVelocity = velocity;


        // Update animator parameters based on networked velocity
        animator.SetFloat("Speed", NetworkVelocity.magnitude / maxSpeed);
    }

    void UpdateSpeed()
    {
        if (HasStateAuthority)
            maxSpeed = CharacterDataManager.Instance.GetCurrentPlayerCharacter().speed;
    }

    private void OnDestroy()
    {
        CharacterDataManager.Instance.OnLevelUp -= UpdateSpeed;
    }

}
