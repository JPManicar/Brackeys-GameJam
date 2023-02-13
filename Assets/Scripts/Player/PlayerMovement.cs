using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ===== Movement Settings =====
    [Header("Horizontal Movement")]

    [SerializeField] float maxSpeed;
    [SerializeField] float airStrafeMaxSpeed;
    [Tooltip("Time to max speed")]
    [SerializeField] float maxSpeedTime;
    [Tooltip("Time to from max speed to stop")]
    [SerializeField] float stopTime;
    [Tooltip("Time to from max speed to max speed in the opposite direction")]
    [SerializeField] float turnTime;
    [SerializeField]
    [ReadOnly]
    float moveAcceleration;
    [SerializeField]
    [ReadOnly]
    float stopAcceleration;
    [SerializeField]
    [ReadOnly]
    float turnAcceleration;

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
        controls = PlayerControlManager.instance;
    }
    private void FixedUpdate()
    {
        Vector2 inputDir = controls.MoveDir;

        // ===== Horizontal movement =====

        // Slow down the player if not pressing any buttons
        if (inputDir.x == 0f)
        {
            if (Mathf.Abs(velocity.x) > Mathf.Abs(stopAcceleration * Time.deltaTime))
                velocity.x = velocity.x + stopAcceleration * Time.deltaTime * (velocity.x > 0f ? 1f : -1f);
            else
                velocity.x = 0f;
        }
        else
        {
            // If moving in the same direction
            if (inputDir.x * velocity.x >= 0)
                velocity.x += moveAcceleration * inputDir.x * Time.deltaTime;
            // Moving in opposite direction / turning
            else
                velocity.x -= turnAcceleration * (inputDir.x) * Time.deltaTime;
        }

        if (groundChecker.IsTouchingTile)
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        else
            velocity.x = Mathf.Clamp(velocity.x, -airStrafeMaxSpeed, airStrafeMaxSpeed);

        // ===== Vertical movement =====

        //float realtime = Time.realtimeSinceStartup;
        float realtime = Time.timeSinceLevelLoad;

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

        rb.velocity = Vector2.zero;
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    private void OnValidate()
    {
        // Limit variables

        float minValue = 0.01f;

        maxSpeed = Mathf.Max(minValue, maxSpeed);
        airStrafeMaxSpeed = Mathf.Max(minValue, airStrafeMaxSpeed);
        maxSpeedTime = Mathf.Max(minValue, maxSpeedTime);
        stopTime = Mathf.Max(minValue, stopTime);
        turnTime = Mathf.Max(minValue, turnTime);
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

        moveAcceleration = maxSpeed / maxSpeedTime;
        stopAcceleration = -maxSpeed / stopTime;
        turnAcceleration = 2f * -maxSpeed / turnTime;

        // Gravity

        gravity = (-2 * maxJumpHeight) / (timeToMaxHeight * timeToMaxHeight);
        fallingGravity = (-2 * maxJumpHeight) / (timeToGround * timeToGround);

        // Jumping

        jumpVelocity = (2f * maxJumpHeight) / timeToMaxHeight;
        minJumpTime = 2 * minJumpHeight / jumpVelocity;
    }
}