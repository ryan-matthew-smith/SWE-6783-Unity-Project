using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer sprite;

    [SerializeField] private LayerMask solidWall;

    [SerializeField] private float moveSpeed = 3f;
    private float dirHori = 1f; // initial direction of x-axis movement

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!onGround() || hitWall())
        {
            dirHori = -dirHori; //flip direction

            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        Vector2 moveVec = new Vector2(dirHori * moveSpeed, rb.velocity.y);
        rb.velocity = moveVec; // commit to movement

        SpriteUpdate();
    }

    private bool hitWall()
    {
        Vector2 dirVec = Vector2.right;
        if (dirHori == -1) dirVec = Vector2.left;

        return (Physics2D.BoxCast(col.bounds.center, col.bounds.size * 0.9f, 0f, dirVec, 0.1f, solidWall));   
    }

    private bool onGround()
    {
        Vector2 pos = new Vector2();
        // Check side in direction of movement to prevent enemy getting stuck.
        if (dirHori == 1) pos = new Vector2(rb.position.x + col.bounds.extents.x, rb.position.y);
        else pos = new Vector2(rb.position.x - col.bounds.extents.x, rb.position.y);
        
        Vector2 size = new Vector2(0.1f, col.bounds.extents.y);
        return Physics2D.BoxCast(pos, size, 0f, Vector2.down, 0.1f, solidWall);
    }

    private void SpriteUpdate()
    {
        if (dirHori > 0) sprite.flipX = true;
        else sprite.flipX = false;
    }
}
