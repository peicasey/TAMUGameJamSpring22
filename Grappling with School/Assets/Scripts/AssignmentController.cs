using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignmentController : MonoBehaviour
{

    // Max horizontal speed
    public float speed;

    // Current horizontal speed
    private float movement;

    // true if the character is facing right
    private bool facingRight;

    // We use this for flipping only the graphics of our charater without changing our hitbox or anything else
    private Transform playerVisual;

    private SpriteRenderer spriteRenderer;
    new private Collider2D collider;
    private Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        playerVisual = transform.GetChild(0);
        facingRight = true;
    }

    // Update is called once per frame
    // Input figuring out and animation
    void Update()
    {
        movement = Input.GetAxis("Horizontal") * speed;

        #region animation

        if ((movement < 0 && facingRight) || (movement > 0 && !facingRight))
            transform.Rotate(0f, 180f, 0f);

        // Updating facingRight
        if (movement > 0)
            facingRight = true;
        if (movement < 0)
            facingRight = false;

        #endregion

    }

    // Physics calculations
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movement, rb.velocity.y);
    }
}
