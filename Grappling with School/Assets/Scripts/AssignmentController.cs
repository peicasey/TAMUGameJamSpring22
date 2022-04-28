using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AssignmentController : MonoBehaviour
{

    #region variables
    // Max horizontal speed
    public float speed;

    // Current horizontal speed
    private float movement;

    // These two values below are mutually exclusive. They can't both be positive at the same time.
    // Value that gets set, and then counts down to 0. When it reaches 0, the assignmnet tries to move
    private float sleepingTime;

    // Value that gets set when the assignmnet decides to move. It counts down, and the assignment stops moving when it gets to 0.
    private float movingTime;


    // Min and max values for the above 2 variables when they get set
    // positive numbers only for these please
    public float minSleepingTime;
    public float maxSleepingTime;
    public float minMovingTime;
    public float maxMovingTime;

    public AudioSource audioPlayer;

    // true if the character is facing right
    private bool facingRight;

    // We use this for flipping only the graphics of our charater without changing our hitbox or anything else
    private Transform assignmentVisual;

    private SpriteRenderer spriteRenderer;
    new private Collider2D collider;
    private Rigidbody2D rb;

    // A list of all the possible sprites we can use for the assignment
    // Randomly pick one when the assignment is put into the world
    [SerializeField]
    private List<Sprite> sprites;

    [SerializeField]
    LayerMask platformLayerMask;

    public Animator mainAnimator;
    private Animator attackAnimator;

    private bool beingPulled = false;

    public GameManager gm;

    private bool dead;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        assignmentVisual = transform.GetChild(0);
        spriteRenderer = assignmentVisual.GetComponent<SpriteRenderer>();
        attackAnimator = assignmentVisual.GetComponent<Animator>();

        facingRight = true;
        audioPlayer = GameObject.Find("Paper death sound").GetComponent<AudioSource>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Tell GameManager that this assignment exists (to add to the count)
        gm.AddAssignment();



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
        // The two values can't both be positive, indicating that the assignment is both moving and standing still
        if (sleepingTime > 0 && movingTime > 0)
        {
            sleepingTime = 0;
        }

        // This way they don't hit the ground running when they fall
        if (!CanMove())
        {
            sleepingTime = GetSleepingTime();
            movingTime = 0;
        }

        // Stop moving before we walk off an edge
        if (movingTime > 0)
        {
            if (!CanMove())
            {
                movingTime = 0;
                sleepingTime = GetSleepingTime();
            }
            // turn around if we get to an edge
            else if (facingRight && !CanMoveRight() && CanMoveLeft())
            {
                facingRight = false;
            }
            else if (!facingRight && !CanMoveLeft() && CanMoveRight())
            {
                facingRight = true;
            }
        }

        // Start moving!
        if ((sleepingTime <= 0 && movingTime <= 0) && CanMove())
        {
            if (CanMoveLeft() && CanMoveRight())
            {
                if (Random.value < 0.5)
                {
                    facingRight = false;
                }
                else
                {
                    facingRight = true;
                }
            }
            else if (CanMoveLeft())
            {
                facingRight = false;
            }
            else
            {
                facingRight = true;
            }

            movingTime = GetMovingTime();
        }


        movement = GetMovement();

        #region animation
        mainAnimator.SetFloat("hrzntalSpeed", Mathf.Abs(movement));

        if (Mathf.Abs(movement) < 0.0000001) {
            attackAnimator.SetBool("isAttacking", false);
        }
        else if ((movement < 0 && facingRight) || (movement > 0 && !facingRight)) {
            assignmentVisual.Rotate(0f, 180f, 0f);
            attackAnimator.SetBool("isAttacking", true);
        }

        #endregion


    }

    // Physics calculations
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movement, rb.velocity.y);

        // Update movingTime and sleepingTime
        if (movingTime > 0)
        {
            movingTime -= Time.deltaTime;
        }

        // One problem: if both values for time are 0, did the assignment just stop moving or stop sleeping?
        // To solve this, check them after changing just one to get the case for going to sleep, and get the case for waking up above
        if (movingTime <= 0 && sleepingTime <= 0)
        {
            sleepingTime = GetSleepingTime();
        }

        if (sleepingTime > 0)
        {
            sleepingTime -= Time.deltaTime;
        }


        if (this.transform.position.y < -100)
        {
            Die();
        }
    }

    // public void Attack() {
    //     Debug.Log("Assignment is attacking");
    //     attackAnimator.SetBool("isAttacking", true);
    // }


    #region wall checks
    // true if the assignment is standing on the ground
    private bool IsGrounded()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        return grounded;
    }

    // true if there is a wall on the assignment's left
    private bool NextToLeftWall()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.left, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        return wall;
    }

    // true if there is a wall on the assignment's right
    private bool NextToRightWall()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.right, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        return wall;
    }

    // true if there is ground to the assignment's left
    private bool IsGroundedLeft()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center - new Vector3(collider.bounds.size.x, 0, 0);
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        return grounded;
    }

    // true if there is ground to the assignment's right
    private bool IsGroundedRight()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center + new Vector3(collider.bounds.size.x, 0, 0);
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        return grounded;
    }

    // true if the assignment can move left
    private bool CanMoveLeft()
    {
        return IsGrounded() && !NextToLeftWall() && IsGroundedLeft();
    }

    // true if the assignment can move right
    private bool CanMoveRight()
    {
        return IsGrounded() && !NextToRightWall() && IsGroundedRight();
    }


    // true if there is at least one possible movement option avaiable right now
    private bool CanMove()
    {

        return (CanMoveLeft() || CanMoveRight()) && !beingPulled;
    }
    #endregion


    // returns the movement that the assignment should be going at
    private float GetMovement()
    {
        if (movingTime <= 0)
        {
            return 0;
        }

        if (facingRight)
        {
            return speed;
        }
        else
        {
            return -speed;
        }
    }


    // returns a random value to set sleepTime to
    private float GetSleepingTime()
    {
        return minSleepingTime + (maxSleepingTime - minSleepingTime) * Random.value;
    }

    // returns a random value to set sleepTime to
    private float GetMovingTime()
    {
        return minMovingTime + (maxMovingTime - minMovingTime) * Random.value;
    }

    public void Pulled()
    {
        beingPulled = true;
    }

    public void Dropped()
    {
        beingPulled = false;
    }

    public void Die()
    {
        if (dead)
        {
            Destroy(this.gameObject);
            return;
        }
        // I had a bug where it died twice for some reason
        dead = true;
        Debug.Log("Assignment died");

        // tell GameManager that assignment died
        gm.RemoveAssignment();
        audioPlayer.Play();
        Destroy(this.gameObject);
    }

    //Detect when the assignment runs into something
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Die();
        }
    }
}
