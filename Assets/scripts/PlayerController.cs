using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Max horizontal speed while grounded.")]
    public float runSpeed = 6f;
    [Tooltip("How fast horizontal input ramps up/down.")]
    public float acceleration = 20f;
    [Tooltip("Extra air control multiplier (0-1).")]
    [Range(0f, 1f)] public float airControl = 0.6f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    [Tooltip("How long after leaving ground you can still jump.")]
    public float coyoteTime = 0.1f;
    [Tooltip("How long before landing a buffered jump is allowed.")]
    public float jumpBufferTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Animation (optional, but recommended)")]
    public Animator animator;
    [Tooltip("Name of the Animator float parameter for horizontal speed.")]
    public string speedParam = "Speed";
    [Tooltip("Name of the Animator bool parameter for grounded state.")]
    public string groundedParam = "IsGrounded";
    [Tooltip("Name of the Animator float parameter for vertical velocity.")]
    public string yVelParam = "YVelocity";
    [Tooltip("Name of the Animator trigger parameter for jump start.")]
    public string jumpTrigger = "Jump";

    Rigidbody2D rb;
    float targetX;          // desired x velocity (from input)
    float lastGroundedTime; // for coyote time
    float lastJumpPressed;  // for jump buffering
    bool isGrounded;
    bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1) Read input
        float inputX = Input.GetAxisRaw("Horizontal"); // -1, 0, 1 from arrow keys

        // 2) Compute desired horizontal velocity
        float control = isGrounded ? 1f : airControl;
        targetX = inputX * runSpeed * control;

        // 3) Jump input (buffered)
        if (Input.GetKeyDown(KeyCode.Space))
            lastJumpPressed = Time.time;

        // 4) Flip sprite to face movement
        if (inputX != 0)
            Face(inputX > 0);
        
        // 5) Animator updates (always “running” on ground)
        if (animator)
        {
            float visualSpeed = Mathf.Abs(isGrounded ? runSpeed : rb.linearVelocity.x);
            animator.SetBool(groundedParam, isGrounded);
            animator.SetFloat(speedParam, visualSpeed);          // keeps running loop on ground
            animator.SetFloat(yVelParam, rb.linearVelocity.y);         // drives jump/fall blends
        }
    }

    void FixedUpdate()
    {
        // Ground check
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded) lastGroundedTime = Time.time;

        // Horizontal movement with smoothing
        float newVX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVX, rb.linearVelocity.y);

        // Handle jump: coyote time + input buffering
        bool canCoyote = Time.time - lastGroundedTime <= coyoteTime;
        bool bufferedJump = Time.time - lastJumpPressed <= jumpBufferTime;

        if (bufferedJump && canCoyote)
        {
            lastJumpPressed = -999f; // consume
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (animator) animator.SetTrigger(jumpTrigger);
        }

        // Small landing snap for animation feel
        if (!wasGrounded && isGrounded)
        {
            // You can fire a "land" trigger here if your Animator has one.
            // animator.SetTrigger("Land");
        }
    }

    void Face(bool right)
    {
        if (facingRight == right) return;
        facingRight = right;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    // Visualize ground check in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
