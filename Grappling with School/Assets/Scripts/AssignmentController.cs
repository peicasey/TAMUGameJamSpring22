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

    // A list of all the possible sprites we can use for the assignment
    // Randomly pick one when the assignment is put into the world
    [SerializeField]
    private List<Sprite> sprites;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        playerVisual = transform.GetChild(0);
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        facingRight = true;


        // PROTIP:
        // This is UnityEngine.Random, not System.Random
        // Keep that in mind when you look stuff up for it

        int index = (int)(Random.value * sprites.Count);
        spriteRenderer.sprite = sprites[index];
    }

    // Update is called once per frame
    // Input figuring out and animation
    void Update()
    {
        movement = Input.GetAxis("Horizontal") * speed;

        #region animation

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
