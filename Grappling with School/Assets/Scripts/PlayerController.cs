using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;
using TMPro;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region variables
    // Max horizontal speed
    public float speed;

    // How high you jump
    public float jumpHeight;

    // How many times you can jump past the first jump
    // Ex: 1 for double jump, 2 for triple jump, 0 for normal
    public int extraJumps;

    // The coyote time thing we talked about
    // If you jump within this long of leaving the floor, it doesn't count against your double jump
    public float coyoteTimeLength;

    // How fast you jump away from walls
    public float horizontalJumpSpeed;

    // true if you want to stick to walls
    public bool wallClimb;

    // How long we ignore user horizontal input and wall stickyness
    public float wallJumpCooldown;

    // The speed lmit for falling down walls
    // Needs to be positive
    public float wallSlideSpeed;

    // How long (time) we want our dash to last
    public float dashTimeLength;

    // How fast we move while in a dash
    public float dashSpeed;

    // How many dashes the user can do after leaving the ground
    public int numDashes;

    // When we do a box cast to determine if we are next to a wall, how far we shave off the top so we don't mistake the moving platform we are standing on for a wall
    // Make this number bigger if there are fast moving vertical platforms in the level
    // EDIT: this isn't Time Leap, there are no moving platforms here, but if it works, why change it?
    public float verticalMargin;


    // The disatnce at which wallclimb becomes active
    // Make this larger if there are fast moving platforms
    public float wallClimbMargin;

    // The distance at which the player is considered grounded
    public float groundMargin;

    // The player's animator
    public Animator animator;

    // The script to activate the timer
    //public ActivePowerupTimer powerUpTimer;
    //public Canvas

    // The game object storing all the checkpoints
    public GameObject checkpoints;

    [SerializeField]
    LayerMask platformLayerMask;

    // true if the character is facing right
    private bool facingRight;

    // We use this for flipping only the graphics of our charater without changing our hitbox or anything else
    private Transform playerVisual;

    private SpriteRenderer spriteRenderer;
    new private Collider2D collider;
    private Rigidbody2D rb;

    // Horizintal velocity
    private float movement;
    private float movementInput;

    // true when the user presses jump and we need to make the character jump
    private bool jumped;

    // Keeps track of how many more times the user can jump
    private int jumpsLeft;

    // Coyote time delay
    private float coyoteTime;

    // Keeps track of a "cooldown" period, where we ignore the user's horizontal input and wall-stickyness
    private float timeAfterWallJump;

    // When we want the player to stay vertically still, we set gravity to 0. This is so we can turn it back on
    private float gravity;

    // true when the user presses dash and we need to make the charater dash
    private bool dashed;

    // How much longer our dash is going to last
    private float timeAfterDash;

    // How many dashes we have left
    private int dashesLeft;

    //private CharacterController controller;

    // The index of where you go when you respawn
    //private int checkpoint;
    private Vector2 startPosition;

    //private bool movementOn = true;

    public float maxHealth = 4;
    private float health;
    [SerializeField]
    private int lives = 4;
    public TextMeshPro healthIndicator;
    public TextMeshProUGUI lifeIndicator;

    public GameManager gm;

    public bool canMove = true;

    public int staticHooksOut = 0;

    [SerializeField]
    private List<GameObject> hookList = new List<GameObject>();

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        playerVisual = transform.GetChild(0);
        facingRight = true;
        gravity = rb.gravityScale;
        //checkpoint = 0;
        //checkpoints.transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("open", true);


        //controller = gameObject.GetComponent<CharacterController>();

        health = maxHealth;
        canMove = true;

        gm = FindObjectOfType<GameManager>();
        startPosition = transform.position;
    }

    /*
    #region input
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // If the user is allowed to jump
        if (!jumped && (jumpsLeft != 0 || IsGrounded()))
        {
            jumped = context.action.triggered;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // Get input for dash
        if (!dashed && timeAfterDash <= 0 && dashesLeft > 0)
        {
            dashed = context.action.triggered;
        }
    }
    #endregion
    */


    // Update is called once per frame
    // Input collection and animation
    void Update()
    {
        healthUIUpdate();

        // If there is floor above and below you
        // Aka you are getting crushed
        if (WallAbove() && WallBelow() && WallAbove() != WallBelow())
        {
            Die();
        }

        if (checkMovementOn()) {
            #region input

            // if we are doing a special ability, we don't want to reset movement just yet
            if (timeAfterWallJump <= 0)
            {
                // When dashing, the ability "fades away" to normal movement.
                float dashMovement = dashSpeed;
                if (!facingRight)
                    dashMovement *= -1;
                movement = /*movementInput*/Input.GetAxis("Horizontal") * speed * (dashTimeLength - timeAfterDash) / dashTimeLength + dashMovement * timeAfterDash / dashTimeLength;

            }
            if (!jumped && (jumpsLeft != 0 || IsGrounded()))
            {
                jumped = Input.GetButtonDown("Jump");
            }

            if (!dashed && timeAfterDash <= 0 && dashesLeft > 0)
            {
                dashed = Input.GetButtonDown("Dash");
            }


            /*
            // This will turn off wall climb if the user is holding down
            if (Input.GetAxis("Vertical") < 0)
            {
                wallClimb = false;
            }
            else
            {
                wallClimb = true;
            }
            */

            // To reset jumpsLeft
            if (IsGrounded())
            {
                jumpsLeft = extraJumps;
                dashesLeft = numDashes;
            }

            #endregion


        #region animation
        
        animator.SetFloat("Hrzntal_Speed", Mathf.Abs(movement));
        /*
        animator.SetBool("IsJumping", jumped);
        animator.SetBool("InAir", !(NextToWall() || IsGrounded()));
        animator.SetBool("OnWall", NextToWall() && !IsGrounded());
        */

            animator.SetFloat("Hrzntal_Speed", Mathf.Abs(movement));
            /*
            animator.SetBool("IsJumping", jumped);
            animator.SetBool("InAir", !(NextToWall() || IsGrounded()));
            animator.SetBool("OnWall", NextToWall() && !IsGrounded());
            */
            if ((movement < 0 && facingRight) || (movement > 0 && !facingRight))
                playerVisual.Rotate(0f, 180f, 0f);

            // Updating facingRight
            if (movement > 0)
                facingRight = true;
            if (movement < 0)
                facingRight = false;

            #endregion

            // I was having a wierd issue where if you hold down move while next to a wall, you don't fall
            // This seems to fix that
            if (timeAfterWallJump <= 0)
            {
                if (NextToLeftHitbox() && movement < 0 || NextToRightHitbox() && movement > 0)
                {
                    movement = 0;
                }
            }
        }

        //For testing purposes
        ///**
        if (Input.GetKeyDown(KeyCode.O))
        {
            Damage();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal();
        }
        //**/
    }

    // Physics calculations
    private void FixedUpdate()
    {
        // Make the wall jump cooldown decrease
        if (timeAfterWallJump > 0)
        {
            timeAfterWallJump -= Time.deltaTime;
        }

        // Make the dash cooldown decrease
        if (timeAfterDash > 0)
        {
            timeAfterDash -= Time.deltaTime;
            if (timeAfterDash < 0)
                timeAfterDash = 0;
        }


        if (IsGrounded())
        {
            coyoteTime = coyoteTimeLength;
        }
        else if (coyoteTime > 0)
        {
            coyoteTime -= Time.deltaTime;
        }

        // Make the user dash
        // Note: if for whatever reason the user dashes and jumps in the same frame, I want the jump to take priority
        if (jumped)
            dashed = false;

        if (dashed)
        {
            timeAfterDash = dashTimeLength;
            movement = dashSpeed;
            if (!facingRight)
                movement *= -1;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(movement, 0);
            if (!IsGrounded())
                dashesLeft--;
        }

        // Make the user go up when they jump, and decrease the jumpsLeft count
        if (jumped)
        {
            if (NextToWall() && wallClimb)
            {
                timeAfterWallJump = wallJumpCooldown;
                if (NextToLeftWall())
                    movement = horizontalJumpSpeed;
                if (NextToRightWall())
                    movement = -horizontalJumpSpeed;

            }
            if (jumpHeight > rb.velocity.y)
            {
                rb.velocity = new Vector2(movement, jumpHeight);
            }
            else
            {
                rb.velocity = new Vector2(movement, rb.velocity.y);
            }
            jumped = false;
            if (!IsGrounded() && coyoteTime <= 0)
            {
                jumpsLeft--;
            }
            coyoteTime = 0;
        }
        else
        {

            if (IsGrounded())
            {
                rb.velocity = new Vector2(movement, rb.velocity.y);
            }
            else
            {
                // The vertical fall speed we can't go over
                float speedLimit = -wallSlideSpeed;

                if ((NextToWall() && timeAfterWallJump <= 0) && wallClimb)
                {
                    // Remember, we are sliding in the negative y direction
                    if (rb.velocity.y < speedLimit)
                    {
                        rb.velocity = new Vector2(movement, speedLimit);
                    }
                    else
                    {
                        rb.velocity = new Vector2(movement, rb.velocity.y);
                    }
                }

                else
                {
                    rb.velocity = new Vector2(movement, rb.velocity.y);
                }
            }
        }

        // If we are dashing
        if (timeAfterDash > 0)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(movement, 0);
        }
        else
        {
            rb.gravityScale = gravity;
        }

        // reset jumped and dashed
        if (jumped)
            jumped = false;

        if (dashed)
            dashed = false;


        if(this.transform.position.y < -100)
        {
            Damage(100);
        }
    }

    #region dying and respawning
    public void Die()
    {
        if (lives > 0) {
            Respawn();
        } else
        {
            GameOver();
        }
    }

    public void Respawn()
    {
        try
        {
            foreach (GameObject i in hookList)
            {
                try
                {
                    i.GetComponent<Hook>().Delete();
                }
                catch
                {
                    Debug.Log("Something went wrong with deleting the hooks individually");
                }
            }
        } catch
        {
            Debug.Log("Something went wrong with going through the hooks");
        }
        transform.position = startPosition;//checkpoints.transform.GetChild(checkpoint).position;

        // reset everything
        rb.velocity = new Vector3(0, 0, 0);
        jumpsLeft = 0;
        timeAfterWallJump = 0;
        dashesLeft = 0;
        timeAfterDash = 0;
        health = maxHealth;
        lives--;

    }
    #endregion

    #region wall checks
    // true if the player is standing on the ground
    private bool IsGrounded()
    {
        float margin = groundMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        if (!grounded)
        {
            raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin);
            grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Movable") : false;
        }

        return grounded;
    }

    private GameObject WallAbove()
    {
        float margin = groundMargin;
        Vector2 point = collider.bounds.center;
        Vector3 box = collider.bounds.size;
        point.y = point.y + box.y * 0.4f;
        box.y = box.y * 0.1f;
        box.x = box.x * 0.5f;  // I did this beause i was having an issue with the launching platform where you wouild sometimes get wedged between the wall and the platform momentarily, so this makes it so you don't die then
                               // Also this makes sense becasue if you are just on the egde you should be pushed out rather than killed
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, box, 0f, Vector2.up, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        if (raycastHit2D.collider)
            return raycastHit2D.collider.CompareTag("Platform") ? raycastHit2D.collider.gameObject : null;
        return null;
    }

    private GameObject WallBelow()
    {
        float margin = groundMargin;
        Vector2 point = collider.bounds.center;
        Vector3 box = collider.bounds.size;
        point.y = point.y - box.y * 0.4f;
        box.y = box.y * 0.1f;
        box.x = box.x * 0.5f;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, box, 0f, Vector2.down, margin, platformLayerMask);

        if (raycastHit2D.collider)
            return raycastHit2D.collider.CompareTag("Platform") ? raycastHit2D.collider.gameObject : null;
        return null;
    }

    // true if there is a wall next to the player
    private bool NextToWall()
    {
        return NextToLeftWall() || NextToRightWall();
    }

    // true if there is a wall on the player's left
    private bool NextToLeftWall()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, BoxSizeVerticalSmaller(), 0f, Vector2.left, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        return wall;
    }

    // true if there is a wall on the player's right
    private bool NextToRightWall()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, BoxSizeVerticalSmaller(), 0f, Vector2.right, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Breakable") : false;

        return wall;
    }

    // To try to fix the problem where holding down move next to a wall
    private bool NextToLeftHitbox()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.left, margin, platformLayerMask);
        bool wall = raycastHit2D.collider;

        return wall;
    }

    private bool NextToRightHitbox()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.right, margin, platformLayerMask);
        bool wall = raycastHit2D.collider;

        return wall;
    }
    #endregion

    // Gets the size of the collider, with the top edges a little shaved off so we don't think the platform above us is a wall we are attached to
    private Vector3 BoxSizeVerticalSmaller()
    {
        float margin = verticalMargin;
        Vector3 box = collider.bounds.size;
        box.y = box.y - 2 * margin;
        return box;
    }


    //Detect when the player runs into something
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("You died!");
            Damage(100);
        }

        if (collision.CompareTag("Assignment"))
        {
            Debug.Log("Touched an assignment");
            // Animator assignmentAnimator1 = collision.GetComponent(typeof(Animator)) as Animator;
            // Animator assignmentAnimator = collision.gameObject.GetComponentInChildren(typeof(Animator)) as Animator;
            // assignmentAnimator.SetBool("isAttacking", true);
            // if(collision.gameObject.GetComponent<AssignmentController>()){
            //     Debug.Log("here");
            //     //collision.gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("isAttacking", true);
            //     //Debug.Log(collision.gameObject.transform.GetChild(0).name);
            //     collision.gameObject.GetComponent<AssignmentController>().Attack();
            //     ///collision.gameObject.transform.GetChild(0).GetComponent<AssignmentController>().Attack();
            //     //checkpoint.GetChild(0).GetComponent<Animator>().SetBool("open", false);
            // }
            Damage();
        }
        /*
        else if (collision.CompareTag("Checkpoint"))
        {
            int index = collision.gameObject.transform.GetSiblingIndex(); // Assumes that the game object we touched is a child of checkpoints
            if (index > checkpoint)
            {
                checkpoint = index;
            }

            collision.gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("open", true);
        }
        */
    }
    /*
    public void setCheckpoint(int check)
    {
        checkpoint = check;
    }

    public void ResetCheckpoints()
    {
        foreach (Transform checkpoint in checkpoints.transform)
        {
            if (checkpoint.GetSiblingIndex() != 0)
            {
                checkpoint.GetChild(0).GetComponent<Animator>().SetBool("open", false);
            }
        }
    }
    */

    public bool checkMovementOn()
    {
        //return !(NextToRightHitbox() || NextToLeftHitbox());
        return canMove;
    }

    public void Damage(float num = 1)
    {
        health -= num;
        if (health <= 0)
        {
            Die();
        }
        //StartCoroutine(ShowDamage());
    }

    // IEnumerator ShowDamage()
    // {
    //     spriteRenderer.color = Color.red; // can't do currently because of body part system; ignore warnings
    //     yield return new WaitForSeconds(1);
    //     spriteRenderer.color = Color.white; 
    // }

    public void Heal(float num = 1)
    {
        if (health <= maxHealth - num)
        {
            health += num;
        } else {
            health = maxHealth;
        }
    }

    private void healthUIUpdate()
    {
        healthIndicator.text = "GPA: " + health + ".0";
        lifeIndicator.text = "Q drops: " + lives;
    }

    private void GameOver()
    {
        gm.GameOver();
        Destroy(this.gameObject);
    }

    public void DeleteThis(HingeJoint2D temp)
    {
        foreach (HingeJoint2D i in this.gameObject.GetComponents<HingeJoint2D>())
        { 
            if (i == temp)
            {
                Debug.Log("Deleting HJ on player");
                Destroy(i);
            }
        }
    }

    public void AddHook(GameObject hook)
    {
        hookList.Add(hook);
    }
    public void RemoveHook(GameObject hook)
    {
        hookList.Remove(hook);
    }

    public float distBtwnHooks()
    {
        if(hookList.Count == 2)
        {
            Debug.Log(Vector3.Distance(hookList[0].GetComponent<Transform>().position, hookList[1].GetComponent<Transform>().position));
            return Vector3.Distance(hookList[0].GetComponent<Transform>().position, hookList[1].GetComponent<Transform>().position);
        }
        return 10;
    }

}