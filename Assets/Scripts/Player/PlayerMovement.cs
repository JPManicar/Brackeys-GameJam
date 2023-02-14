using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Serializable]
    class MovementType
    {
        // For now is just helping for debugging
        public enum Type 
        {
            WALKING,
            SPRINTING,
            DASHING,
            OTHERS, // Shouldn't have
        }

        public Type type;
        public float maxSpeed;
        [Tooltip("Speed when moving in the air")]
        public float airStrafeMaxSpeed;
        [Tooltip("Time to max speed")]
        public float maxSpeedTime;
        [Tooltip("Time to from max speed to stop")]
        public float stopTime;
        [Tooltip("Time to from max speed to max speed in the opposite direction")]
        public float turnTime;
        // Put as -float.MinValue if want to transition immediately
        public float transitionStartTiming;
        public float transitionEndTiming;

        [ReadOnly] public float moveAcceleration;
        [ReadOnly] public float stopAcceleration;
        [ReadOnly] public float turnAcceleration;


        // Modifies an existing object to prevent recreating a new object everytime need to lerp
        public void Lerp(MovementType initialValue, MovementType targetValue, float t)
        {
            print($"{initialValue.maxSpeed}   |   {targetValue.maxSpeed}   |  T: " + t);
            maxSpeed = Mathf.Lerp(initialValue.maxSpeed, targetValue.maxSpeed, t);
            airStrafeMaxSpeed = Mathf.Lerp(initialValue.airStrafeMaxSpeed, targetValue.airStrafeMaxSpeed, t);
            maxSpeedTime = Mathf.Lerp(initialValue.maxSpeedTime, targetValue.maxSpeedTime, t);
            stopTime = Mathf.Lerp(initialValue.stopTime, targetValue.stopTime, t);
            turnTime = Mathf.Lerp(initialValue.turnTime, targetValue.turnTime, t);
            moveAcceleration = Mathf.Lerp(initialValue.moveAcceleration, targetValue.moveAcceleration, t);
            stopAcceleration = Mathf.Lerp(initialValue.stopAcceleration, targetValue.stopAcceleration, t);
            turnAcceleration = Mathf.Lerp(initialValue.turnAcceleration, targetValue.turnAcceleration, t);
        }

        // Deep copy from original to this
        public static void Clone(MovementType destination, MovementType source)
        {
            destination.type = source.type;
            destination.maxSpeed = source.maxSpeed;
            destination.airStrafeMaxSpeed = source.airStrafeMaxSpeed;
            destination.maxSpeedTime = source.maxSpeed;
            destination.stopTime = source.stopTime;
            destination.turnTime = source.turnTime;
            destination.transitionStartTiming = source.transitionStartTiming;
            destination.transitionEndTiming = source.transitionEndTiming;
            destination.moveAcceleration = source.moveAcceleration;
            destination.stopAcceleration = source.stopAcceleration;
            destination.turnAcceleration = source.turnAcceleration;
        }

        // Deep copy from original to this
        public MovementType(MovementType source)
        {
            type = source.type;
            maxSpeed = source.maxSpeed;
            airStrafeMaxSpeed = source.airStrafeMaxSpeed;
            maxSpeedTime = source.maxSpeed;
            stopTime = source.stopTime;
            turnTime = source.turnTime;
            transitionStartTiming = source.transitionStartTiming;
            transitionEndTiming = source.transitionEndTiming;
            moveAcceleration = source.moveAcceleration;
            stopAcceleration = source.stopAcceleration;
            turnAcceleration = source.turnAcceleration;
        }
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

    // ===== Game Jam Specific, remove all game jam =====
    public float overrideMaxSpeed;
    public bool ifOverrideMaxSpeed;

    // ===== Private variables =====

    private PlayerControlManager controls;

    private Rigidbody2D rb;

    // Variables to track the state of the player's movement and input
    [Header("Tracking Variables")]
    
    [SerializeField]
    [ReadOnly]
    MovementType targetMovementType;
    [SerializeField]
    [ReadOnly]
    MovementType currMovementType;

    Coroutine movementTransitionCoroutine;

    [SerializeField]
    [ReadOnly]
    Vector2 velocity;

    // If the last facing direction is left, for stationary dashing
    // TODO: Change when animation is applied
    [SerializeField]
    [ReadOnly]
    bool isLastFacingDirLeft; 

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

        MovementType.Clone(currMovementType, walking);
    }

    private void FixedUpdate()
    {
        Vector2 inputDir = controls.MoveDir;

        UpdateMovementTypeState();
        HorizontalMovement(inputDir.x);
        VerticalMovement(inputDir.y);

        rb.velocity = Vector2.zero;
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    // ========== Custom Functions ==========
    private void UpdateMovementTypeState()
    {
        if (Time.time - lastDashedTime < dashDuration) // If still dashing, no need check if player is pressing
            SetMovementType(dashMovement, false);
        else if (controls.IsSprinting)
            SetMovementType(sprintMovement, true);
        else
            SetMovementType(walking, true);
    }

    private void SetMovementType(MovementType targetMovementType, bool ifTransition = true)
    {
        // Return if its already transitioning to that type
        if (this.targetMovementType.type == targetMovementType.type)
            return;

        if (ifTransition)
        {
            if (movementTransitionCoroutine != null)
                StopCoroutine(movementTransitionCoroutine);

            this.targetMovementType = targetMovementType;
            movementTransitionCoroutine = StartCoroutine(ChangeMovementTypeCoroutine());
        }
        else
        {
            this.targetMovementType = targetMovementType;
            MovementType.Clone(currMovementType, targetMovementType);
        }
    }

    private IEnumerator ChangeMovementTypeCoroutine()
    {
        MovementType prevMovementType = new MovementType(currMovementType);
        float t = 0f;
        float transitionTime = prevMovementType.transitionEndTiming + targetMovementType.transitionStartTiming;

        while (t < transitionTime)
        {
            t += Time.deltaTime;
            currMovementType.Lerp(prevMovementType, targetMovementType, t / transitionTime);
            yield return null;
        }

        MovementType.Clone(currMovementType, targetMovementType);
        movementTransitionCoroutine = null;
    }
    
    private void HorizontalMovement(float xInput) 
    {
        // Force input to 1 if dashing
        if (currMovementType.type == MovementType.Type.DASHING)
            xInput = isLastFacingDirLeft ? -1 : 1;

        // Slow down the player if not pressing any buttons
        if (xInput == 0f)
        {
            if (Mathf.Abs(velocity.x) > Mathf.Abs(currMovementType.stopAcceleration * Time.deltaTime))
                velocity.x = velocity.x + currMovementType.stopAcceleration * Time.deltaTime * (velocity.x > 0f ? 1f : -1f);
            else
                velocity.x = 0f;
        }
        else
        {
            // If moving in the same direction
            if (xInput * velocity.x >= 0)
                velocity.x += currMovementType.moveAcceleration * xInput * Time.deltaTime;
            // Moving in opposite direction / turning
            else
                velocity.x -= currMovementType.turnAcceleration * (xInput) * Time.deltaTime;
        }

        if (ifOverrideMaxSpeed)
            velocity.x = Mathf.Clamp(velocity.x, -overrideMaxSpeed, overrideMaxSpeed);
        else if (groundChecker.IsTouchingTile)
            velocity.x = Mathf.Clamp(velocity.x, -currMovementType.maxSpeed, currMovementType.maxSpeed);
        else
            velocity.x = Mathf.Clamp(velocity.x, -currMovementType.airStrafeMaxSpeed, currMovementType.airStrafeMaxSpeed);

        if (velocity.x < 0)
            isLastFacingDirLeft = true;
        else if (velocity.x > 0)
            isLastFacingDirLeft = false;
    }

    private void VerticalMovement(float inputY)
    {
        if (groundChecker.IsTouchingTile && velocity.y <= 0f)
        {
            lastJumpTime = float.MinValue;
            lastGroundedTime = Time.time;

            velocity.y = 0f;
        }
        else if (controls.IsJumping)
        {
            lastJumpPressed = Time.time;
        }

        bool JumpBuffer() => Time.time - lastJumpPressed < jumpBuffer;
        bool CoyoteTime() => Time.time - lastGroundedTime < coyoteTime;

        if ((JumpBuffer() || controls.IsJumping) &&
            ifReleaseJumpAfterJumping &&
            CoyoteTime())
        {
            velocity.y = jumpVelocity;
            lastJumpPressed = float.MinValue; // Prevent jump buffer from triggering again
            lastGroundedTime = float.MinValue;
            lastJumpTime = Time.time;
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
            else if (!controls.IsJumping && (Time.time - lastJumpTime) > minJumpTime)
                velocity.y += gravity * Time.deltaTime * gravityMultiplierWhenRelease;
            else
                velocity.y += gravity * Time.deltaTime;
        }
        else if (groundChecker.IsTouchingTile)
            velocity.y = 0f;
        else
            velocity.y += fallingGravity * Time.deltaTime;

        velocity.y = Mathf.Max(velocity.y, maxFallVelocity);
    }

    // ========== Events ==========
    private void OnDash(InputAction.CallbackContext context)
    {
        // If player press dash and cooldown is over
        if (Time.timeSinceLevelLoad - lastDashedTime < dashCooldown)
            return;

        SetMovementType(dashMovement, false);
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