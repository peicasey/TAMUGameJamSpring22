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

    [SerializeField]
    LayerMask platformLayerMask;

    // TEMPORARY - these 2 variables are not intended for final product
    private SpriteRenderer leftBox;
    private SpriteRenderer rightBox;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        playerVisual = transform.GetChild(0);
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
        facingRight = true;


        // TEMPORARY
        leftBox = transform.GetChild(1).GetComponent<SpriteRenderer>();
        rightBox = transform.GetChild(2).GetComponent<SpriteRenderer>();


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


        // FOR TESTING PURPOSES
        // Not intended for final product

        // For now, there are 2 boxes on either side of the assignment representing the area it is checking
        // This code will change the color of those boxes to show us what it detects

        if (IsGrounded()) {
            if (NextToLeftWall()) {
                leftBox.color = new Color(1, 0, 0, 0.4f);
            }
            else {
                if (IsGroundedLeft()) {
                    leftBox.color = new Color(0, 1, 0, 0.4f);
                }
                else {
                    leftBox.color = new Color(1, 1, 0, 0.4f);
                }
            }


            if (NextToRightWall()) {
                rightBox.color = new Color(1, 0, 0, 0.4f);
            }
            else {
                if (IsGroundedRight()) {
                    rightBox.color = new Color(0, 1, 0, 0.4f);
                }
                else {
                    rightBox.color = new Color(1, 1, 0, 0.4f);
                }
            }
        }
        else
        {
            leftBox.color = new Color(0, 0, 0, 0.4f);
            rightBox.color = new Color(0, 0, 0, 0.4f);
        }
    }

    // Physics calculations
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movement, rb.velocity.y);
    }


    #region wall checks
    // true if the assignment is standing on the ground
    private bool IsGrounded()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

        return grounded;
    }

    // true if there is a wall on the assignment's left
    private bool NextToLeftWall()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.left, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

        return wall;
    }

    // true if there is a wall on the assignment's right
    private bool NextToRightWall()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.right, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

        return wall;
    }

    // true if there is ground to the assignment's left
    private bool IsGroundedLeft()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center - new Vector3(collider.bounds.size.x, 0, 0);
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

        return grounded;
    }

    // true if there is ground to the assignment's right
    private bool IsGroundedRight()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center + new Vector3(collider.bounds.size.x, 0, 0);
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

        return grounded;
    }
    #endregion
}
