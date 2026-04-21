using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Handles player movement including walking and running.
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeedPercentage = 0.3f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float deceleration = 2f;

    SimpleKCC characterController;
    Animator animator;
    CharacterLook rotateCharacter;

    float maxSpeed = 5f;
    Vector3 moveDirection;
    Vector3 velocity;

    [Networked] Vector3 NetworkVelocity { get; set; }

    bool isWalking = false;

    // -- Initialization --

    private void Start()
    {
        characterController = GetComponent<SimpleKCC>();
        animator = GetComponentInChildren<Animator>();
        rotateCharacter = GetComponent<CharacterLook>();
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

        // Get input data from the network
        if (GetInput(out NetworkInputData data))
        {
            moveDirection = new Vector3(data.Move.x, 0, data.Move.y);
            isWalking = data.Walk;
        }

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
