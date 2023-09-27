using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;

    private SpriteRenderer sprite;
    private float dirHori = 0f;
    [SerializeField] private float moveSpeed = 6f; //[SerializeField] lets you edit value in the unity editor
    [SerializeField] private float jumpForce = 12f;
    private bool isJumping = false;
    private float hitForce = 6f;


    private enum MovementState
    {
        idle,
        run,
        jump,
        fall
    }

    // Start is called before the first frame update
    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        //anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirHori = Input.GetAxisRaw("Horizontal"); //GetAxisRaw (as opposed to GetAxis()) drops axis to 0 immediately (makes player stop moving after release)

        rb.velocity = new Vector2(dirHori * moveSpeed, rb.velocity.y); // create movement vector based on axis input

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            StartJump();
        }

        // Variable jump height code
        if (isJumping) 
        {
            if (rb.velocity.y >= 0f && !Input.GetButton("Jump"))
            {
                // cut player y velocity if jump key is released and still moving upwards
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
                EndJump();
            }
            else if (rb.velocity.y <= 0f && IsGrounded())
            {
                EndJump();
            }
        }

        //UpdateAnimationState();

    }

    public void StartJump()
    {
        isJumping = true;
        Debug.Log("JUMP START");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    // Ends player jump and increases gravity
    public void EndJump()
    {
        isJumping = false;
        Debug.Log("JUMP END");
    }

    // Launches player upwards slightly on hazard collision
    public void hitJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, hitForce);
    }

    private void UpdateAnimationState()
    {
        MovementState state;
        // Animation
        if (dirHori < 0f) // Running left
        {
            sprite.flipX = true;
            state = MovementState.run;
        }
        else if (dirHori > 0f) // Running right
        {
            sprite.flipX = false;
            state = MovementState.run;
        }
        else // Idle
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f) // Jumping up
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -0.1f) // Falling down
        {
            state = MovementState.fall;
        }

        // Sets animation state variable within Unity
        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        // returns true if collision with jumpableGround LayerMask is found
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
