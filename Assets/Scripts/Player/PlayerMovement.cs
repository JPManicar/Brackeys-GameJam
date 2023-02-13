using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.ScrollRect;

public class PlayerMovement : MonoBehaviour
{
    [Serializable]
    class MovementType
    {
        [SerializeField] public float maxSpeed;
        [Tooltip("Speed when moving in the air")]
        [SerializeField] public float airStrafeMaxSpeed;
        [Tooltip("Time to max speed")]
        [SerializeField] public float maxSpeedTime;
        [Tooltip("Time to from max speed to stop")]
        [SerializeField] public float stopTime;
        [Tooltip("Time to from max speed to max speed in the opposite direction")]
        [SerializeField] public float turnTime;
        [SerializeField]
        [ReadOnly]
        public float moveAcceleration;
        [SerializeField]
        [ReadOnly]
        public float stopAcceleration;
        [SerializeField]
        [ReadOnly]
        public float turnAcceleration;
    }

    // ===== Movement Settings =====
    [Header("Horizontal Movement")]
    [SerializeField] MovementType walking;
    [SerializeField] MovementType sprintMovement;
    [SerializeField] MovementType dashMovement;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;

    [Header("Gravity")]

    [SerializeField] float maxFallVelocity;
    [Tooltip("Gravity when the player is moving up. Control by changing [Max Jump Height] and [Time To Max Height]")]
    [SerializeField]
    [ReadOnly]
    float gravity;
    [Tooltip("Gravity when the player is falling. Control by changing [Max Jump Height] and [Time To Ground]")]
    [SerializeField]
    [ReadOnly]
    float fallingGravity;

    [Header("Jumping")]

    [SerializeField] float maxJumpHeight;
    [SerializeField] float minJumpHeight;
    [SerializeField]
    [ReadOnly]
    float minJumpTime;
    [SerializeField] float timeToMaxHeight;
    [Tooltip("Time from max height to ground (y = 0)")]
    [SerializeField] float timeToGround;
    [Tooltip("Upwards velocity when the player press the jump button")]
    [SerializeField]
    [ReadOnly]
    float jumpVelocity;
    [SerializeField]
    [Tooltip("Increased gravity when the player is moving up and the jump button is not pressed. Helps player drop down faster after releasing jump")]
    float gravityMultiplierWhenRelease;
    [Tooltip("Let the player jump even if they just fell off a platform")]
    [SerializeField] float coyoteTime;
    [Tooltip("Let the player queue the next jump if the player jumped in mid air.")]
    [SerializeField] float jumpBuffer;

    [Header("Others")]

    [SerializeField] TileChecker groundChecker;
    [SerializeField] TileChecker ceilingChecker;

    // ===== Private variables =====

    private PlayerControlManager controls;

    private Rigidbody2D rb;

    // Variables to track the state of the player's movement and input
    [Header("Tracking Variables")]

    MovementType currMovementType;

    [SerializeField]
    [ReadOnly]
    Vector2 velocity;

    [SerializeField]
    [ReadOnly]
    float lastJumpPressed;
    [SerializeField]
    [ReadOnly]
    float lastGroundedTime;
    [SerializeField]
    [ReadOnly]
    float lastJumpTime;
    [SerializeField]
    [ReadOnly]
    float lastDashedTime;

    // If the player has released the jump button after jumping
    [SerializeField]
    [ReadOnly]
    bool ifReleaseJumpAfterJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        velocity = Vector2.zero;

        lastJumpPressed = float.MinValue;
        lastGroundedTime = float.MinValue;
        lastJumpTime = float.MinValue;
        lastDashedTime = float.MinValue;

        controls = PlayerControlManager.instance;
        controls.OnDash += OnDash;
    }

    private void FixedUpdate()
    {
        //float realtime = Time.realtimeSinceStartup;
        float realtime = Time.timeSinceLevelLoad;

        Vector2 inputDir = controls.MoveDir;

        if (realtime - lastDashedTime < dashDuration) // If still dashing, no need check if player is pressing
            currMovementType = dashMovement;
        else if (controls.IsSprinting)
            currMovementType = sprintMovement;
        else
            currMovementType = walking;

        if (currMovementType == dashMovement)
            print("Dashing");
        else if (currMovementType == sprintMovement)
            print("Sprinting");
        else
            print("Walking");

        // ===== Horizontal movement =====

        // Slow down the player if not pressing any buttons
        if (inputDir.x == 0f)
        {
            if (Mathf.Abs(velocity.x) > Mathf.Abs(currMovementType.stopAcceleration * Time.deltaTime))
                velocity.x = velocity.x + currMovementType.stopAcceleration * Time.deltaTime * (velocity.x > 0f ? 1f : -1f);
            else
                velocity.x = 0f;
        }
        else
        {
            // If moving in the same direction
            if (inputDir.x * velocity.x >= 0)
                velocity.x += currMovementType.moveAcceleration * inputDir.x * Time.deltaTime;
            // Moving in opposite direction / turning
            else
                velocity.x -= currMovementType.turnAcceleration * (inputDir.x) * Time.deltaTime;
        }

        if (groundChecker.IsTouchingTile)
            velocity.x = Mathf.Clamp(velocity.x, -currMovementType.maxSpeed, currMovementType.maxSpeed);
        else
            velocity.x = Mathf.Clamp(velocity.x, -currMovementType.airStrafeMaxSpeed, currMovementType.airStrafeMaxSpeed);

        // ===== Vertical movement =====

        if (groundChecker.IsTouchingTile && velocity.y <= 0f)
        {
            lastJumpTime = float.MinValue;
            lastGroundedTime = realtime;

            velocity.y = 0f;
        }
        else if (controls.IsJumping)
        {
            lastJumpPressed = realtime;
        }

        bool JumpBuffer() => realtime - lastJumpPressed < jumpBuffer;
        bool CoyoteTime() => realtime - lastGroundedTime < coyoteTime;

        if ((JumpBuffer() || controls.IsJumping) &&
            ifReleaseJumpAfterJumping &&
            CoyoteTime())
        {
            velocity.y = jumpVelocity;
            lastJumpPressed = float.MinValue; // Prevent jump buffer from triggering again
            lastGroundedTime = float.MinValue;
            lastJumpTime = realtime;
            ifReleaseJumpAfterJumping = false;
        }
        if (!controls.IsJumping)
        {
            ifReleaseJumpAfterJumping = true;
        }

        // Gravity
        if (velocity.y > 0f)
        {
            if (ceilingChecker.IsTouchingTile)
                velocity.y = 0f;
            // Increased gravity if player is moving up and not inputting jump
            // Also check if it jumped to the minimum amount of seconds
            else if (!controls.IsJumping && (realtime - lastJumpTime) > minJumpTime)
                velocity.y += gravity * Time.deltaTime * gravityMultiplierWhenRelease;
            else
                velocity.y += gravity * Time.deltaTime;
        }
        else if (groundChecker.IsTouchingTile)
            velocity.y = 0f;
        else
            velocity.y += fallingGravity * Time.deltaTime;

        velocity.y = Mathf.Max(velocity.y, maxFallVelocity);

        rb.velocity = Vector2.zero;
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    // ========== Events ==========
    private void OnDash(InputAction.CallbackContext context)
    {
        // If player press dash and cooldown is over
        if (Time.timeSinceLevelLoad - lastDashedTime < dashCooldown)
            return;

        currMovementType = dashMovement;
        lastDashedTime = Time.timeSinceLevelLoad;
    }

    private void OnValidate()
    {
        // Limit variables

        float minValue = 0.01f;

        MovementType[] movementTypes = { walking, sprintMovement, dashMovement };
        foreach (MovementType type in movementTypes)
        {
            type.maxSpeed = Mathf.Max(minValue, type.maxSpeed);
            type.airStrafeMaxSpeed = Mathf.Max(minValue, type.airStrafeMaxSpeed);
            type.maxSpeedTime = Mathf.Max(minValue, type.maxSpeedTime);
            type.stopTime = Mathf.Max(minValue, type.stopTime);
            type.turnTime = Mathf.Max(minValue, type.turnTime);
        }
        dashDuration = Mathf.Max(-minValue, dashDuration);
        dashCooldown = Mathf.Max(-minValue, dashCooldown);
        maxFallVelocity = Mathf.Min(-minValue, maxFallVelocity);
        maxJumpHeight = Mathf.Max(minValue, maxJumpHeight);
        minJumpHeight = Mathf.Max(minValue, minJumpHeight);
        timeToMaxHeight = Mathf.Max(minValue, timeToMaxHeight);
        timeToGround = Mathf.Max(minValue, timeToGround);
        gravityMultiplierWhenRelease = Mathf.Max(minValue, gravityMultiplierWhenRelease);
        coyoteTime = Mathf.Max(minValue, coyoteTime);
        jumpBuffer = Mathf.Max(minValue, jumpBuffer);

        // Calculate physics before hand, prevent runtime calculation
        // Movement

        foreach (MovementType type in movementTypes)
        {
            type.moveAcceleration = type.maxSpeed / type.maxSpeedTime;
            type.stopAcceleration = -type.maxSpeed / type.stopTime;
            type.turnAcceleration = 2f * -type.maxSpeed / type.turnTime;
        }

        // Gravity

        gravity = (-2 * maxJumpHeight) / (timeToMaxHeight * timeToMaxHeight);
        fallingGravity = (-2 * maxJumpHeight) / (timeToGround * timeToGround);

        // Jumping

        jumpVelocity = (2f * maxJumpHeight) / timeToMaxHeight;
        minJumpTime = 2 * minJumpHeight / jumpVelocity;
    }
}