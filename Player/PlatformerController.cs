using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlatformerController : MonoBehaviour
{
    [Header("VFX / SFX Hooks")]
    public UnityEvent onJump;
    public UnityEvent onLand;
    //public UnityEvent onRespawn;

    [Header("Visuals")]
    public Transform mesh;
    public float meshFlipSpeed = 10f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip land;
    public AudioClip jump;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpHeight = 4f;
    public float gravity = -20f;
    public float groundCheckDistance = 0.1f;

    [Header("Time Loop Settings")]
    public KeyCode resetKey = KeyCode.F;
    public KeyCode fullResetKey = KeyCode.R;
    public float resetDelay = 0.5f;

    [Header("Ground Detection")]
    public LayerMask groundLayerMask = 1; // Include ghost layer if needed

    [Header("Jump Forgiveness")]
    [Tooltip("Time window after leaving ground when a jump is still allowed.")]
    public float coyoteTime = 0.2f;
    [Tooltip("Time window before landing when a jump input is buffered.")]
    public float jumpBufferTime = 0.15f;

    [Header("States")]
    public KeyCode crouchKey = KeyCode.S;

    private CharacterController controller;
    private Renderer[] renderers;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private float horizontalInput;

    // mesh flip rotations
    private Quaternion meshLeftRotation;
    private Quaternion meshRightRotation;

    // Forgiveness timers
    private float coyoteTimer;
    private float jumpBufferCounter;
    private bool isResetting = false;

    // Fallback start values (in case no checkpoints are active)
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {   
        audioSource = GetComponent<AudioSource>();  
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError("CharacterController component is required!");

        renderers = GetComponentsInChildren<Renderer>();

        // capture original spawn
        startPosition = transform.position;
        startRotation = transform.rotation;

        // cache mesh rotations if provided
        if (mesh != null)
        {
            meshLeftRotation = mesh.rotation;
            meshRightRotation = meshLeftRotation * Quaternion.Euler(0, 180f, 0);
        }

        // sanity-check checkpoint manager
        if (CheckpointManager.Instance == null)
            Debug.LogWarning("No CheckpointManager found in scene – resets will fall back to original spawn.");
    }

    void Update()
    {
        HandleTimeLoop();
        if (isResetting) return;

        HandleInput();
        CheckGrounded();
        CheckCeiling();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        ApplyGravity();
        MoveCharacter();
    }

    void HandleTimeLoop()
    {
        // prevent multiple resets while one is in progress
        if (isResetting) return;

        if (Input.GetKeyDown(resetKey))
        {
            StartCoroutine(ResetWithDelay());
        }


    }

    IEnumerator ResetWithDelay()
    {
        isResetting = true;
        CreateGhost();
        SetPlayerVisible(false);

        yield return new WaitForSeconds(resetDelay);

        // pick next respawn
        Transform respawn = null;
        if (CheckpointManager.Instance != null)
            respawn = CheckpointManager.Instance.GetNextRespawnPoint();

        if (respawn != null)
        {
            controller.enabled = false;
            transform.position = respawn.position;
            transform.rotation = respawn.rotation;
            controller.enabled = true;
        }
        else
        {
            // no checkpoints yet? original start
            ResetToStart();
        }

        SetPlayerVisible(true);
        velocity = Vector3.zero;
        isResetting = false;
    }

    void SetPlayerVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isCrouching = Input.GetKey(crouchKey);

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    void CheckGrounded()
    {
        Vector3 rayStart = transform.position;
        float rayDistance = (controller.height / 2f) + groundCheckDistance;

        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, rayDistance, groundLayerMask);

        if (!wasGrounded && isGrounded)
            onLand?.Invoke();
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void HandleMovement()
    {
        velocity.x = horizontalInput * moveSpeed;
        FlipMesh();
    }

    void FlipMesh()
    {
        if (mesh == null || Mathf.Approximately(horizontalInput, 0f)) return;

        var target = horizontalInput > 0 ? meshRightRotation : meshLeftRotation;
        mesh.rotation = Quaternion.Lerp(mesh.rotation, target, Time.deltaTime * meshFlipSpeed);
    }

    void HandleJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimer > 0f)
        {
            PlayJump();
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f;
            coyoteTimer = 0f;

            onJump?.Invoke();
        }
    }

    void HandleCrouch()
    {
        if (isCrouching)
            velocity.x *= 0.5f;
    }

    void ApplyGravity()
    {
        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
    }

    void MoveCharacter()
    {
        controller.Move(velocity * Time.deltaTime);
    }

    void CreateGhost()
    {
        if (GhostManager.Instance == null)
        {
            Debug.LogError("GhostManager.Instance is null! Cannot create ghost.");
            return;
        }

        var stateType = DetermineCurrentState();
        var state = new GhostState(
            stateType,
            transform.position,
            transform.rotation,
            velocity,
            isGrounded,
            horizontalInput
        );

        GhostManager.Instance.CreateGhost(transform.position, transform.rotation, state);
    }

    GhostState.StateType DetermineCurrentState()
    {
        if (isCrouching) return GhostState.StateType.Crouching;
        if (!isGrounded && velocity.y > 0) return GhostState.StateType.Jumping;
        if (!isGrounded && velocity.y < 0) return GhostState.StateType.Falling;
        if (Mathf.Abs(horizontalInput) > 0.1f) return GhostState.StateType.Moving;
        return GhostState.StateType.Standing;
    }

    public void ResetToStart()
    {
        controller.enabled = false;
        transform.position = startPosition;
        transform.rotation = startRotation;
        controller.enabled = true;

        velocity = Vector3.zero;
        isGrounded = false;
        isCrouching = false;

        Debug.Log("Player reset to start position");
    }

    public void Launch(Vector3 launchVelocity)
    {
        velocity.y = 0;
        velocity += launchVelocity;
    }

    void OnDrawGizmosSelected()
    {
        if (controller != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 rayStart = transform.position;
            float rayDistance = (controller.height / 2f) + groundCheckDistance;
            Gizmos.DrawRay(rayStart, Vector3.down * rayDistance);
        }
    }
    void CheckCeiling()
    {
        Vector3 rayStart = transform.position + Vector3.up * (controller.height / 2f);
        float rayDistance = 0.1f; // Small buffer to catch head bumps

        if (Physics.Raycast(rayStart, Vector3.up, rayDistance, groundLayerMask))
        {
            if (velocity.y > 0)
                velocity.y = 0f;
        }
    }

    public void playLand()
    {
        audioSource.PlayOneShot(land);
    }
    public void PlayJump()
    {
        audioSource.PlayOneShot(jump);
    }
}