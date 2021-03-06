using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hook : MonoBehaviour
{
    private Vector3 shootDir;
    private Rigidbody2D rbHook;
    [SerializeField] private float force;
    [SerializeField] private bool isHook1;
    [SerializeField] private bool beingShot;
    private FixedJoint2D fj;
    public float dist = (float)0.2;
    GameObject p1;

    private bool pulling;
    private GameObject targetObj;
    private bool hasHooked = false;
    [SerializeField]
    private SpriteRenderer sprite;
    private Vector3 startPos;

    public GameObject ch;
    private GameObject currentChain;

    public float RetractToLength;

    public float maxTravelDistance;
    public Vector3 currentPos;

    private Transform firePoint;
    private Shoot st;

    public AudioSource audioPlayer;

    private void Start()
    {
        p1 = GameObject.FindGameObjectWithTag("Player");
        rbHook = GetComponent<Rigidbody2D>();
        fj = this.gameObject.AddComponent<FixedJoint2D>();
        fj.enabled = false;
        currentChain = Instantiate<GameObject>(ch);

        currentChain.GetComponent<Chain>().target1 = gameObject;
        currentChain.GetComponent<Chain>().target2 = p1;
        currentChain.GetComponent<Chain>().anchor = firePoint;
        Debug.Log(currentChain.GetComponent<Chain>().target1 + "" + currentChain.GetComponent<Chain>().target2);
    }

    private void Update()
    {
        if (CheckMaxDist() && beingShot)
        {
            Debug.Log("Hook has exceeded max distance");
            Delete();
        }
    }

    void OnDrawGizmos()
    {
        Debug.Log("Drawing Gizmos");
        Gizmos.DrawSphere(firePoint.position, (float)0.1);
    }

    public bool CheckMaxDist()
    {
        currentPos = this.transform.position;
        return (currentPos - startPos).magnitude > maxTravelDistance;
    }
    
    public void Setup(Shoot shooter, Vector3 shootDir, bool isArm1, Transform firePt)
    {
        st = shooter;
        Debug.Log("The hook is being shot");
        beingShot = true;
        this.shootDir = shootDir;
        this.isHook1 = isArm1;
        sprite.color = isHook1 ? Color.blue : Color.red;
        //rbHook.velocity = shootDir * force;

        firePoint = firePt;
        startPos = firePoint.position;
    }

    public void Retract()
    {
        Delete();
    }

    public void Delete()
    {
        if (rbHook.bodyType == RigidbodyType2D.Static)
            p1.GetComponent<PlayerController>().staticHooksOut -= 1;

        Debug.Log("Hook: Deleting hook");
        if (fj.enabled)
        {
            Drop(targetObj);
        }
        else
        {
            DisconnectRope();
        }
        beingShot = false;

        if (currentChain) { 
            p1.GetComponent<PlayerController>().DeleteThis(currentChain.GetComponent<Chain>().getHJ(2));
            currentChain.GetComponent<Chain>().Delete();
        }

        p1.GetComponent<PlayerController>().RemoveHook(this.gameObject);
        Destroy(this.gameObject);
    }
    public void ConnectRope()
    {

        p1.GetComponent<Grapple>().startGrapple(isHook1, this.gameObject);
    }

    public void DisconnectRope()
    {
        Debug.Log("Disconnect: Retracting hook");
        p1.GetComponent<Grapple>().endGrapple(isHook1, this.gameObject);
        st.Aiming();
    }

    /**
     * Bad version of trying to do this
    public void ConnectRope()
    {
        GameObject[] arms = GameObject.FindGameObjectsWithTag("Arm");
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].GetComponent<Rope>().enableRope(isHook1, this.transform);
        }
    }

    public void DisconnectRope()
    {
        GameObject[] arms = GameObject.FindGameObjectsWithTag("Arm");
        for(int i = 0; i < arms.Length; i++)
        {
            arms[i].GetComponent<Rope>().disableRope(isHook1, this.transform);
        }
    }
    **/

    private void FixedUpdate()
    {
        currentPos = this.transform.position;
        if (beingShot)
        {
            rbHook.velocity = shootDir * force;
        }

        if (p1.GetComponent<PlayerController>().staticHooksOut == 2)
        {
            currentChain.GetComponent<Chain>().ExtendTo(p1.GetComponent<PlayerController>().distBtwnHooks() / 2);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (canHook() && !CheckMaxDist())
        {
            if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Breakable"))
            {
                audioPlayer.Play();

                p1.GetComponent<PlayerController>().AddHook(this.gameObject);
                //ConnectRope();
                shootDir = Vector3.zero;
                rbHook.velocity = Vector3.zero;
                rbHook.bodyType = RigidbodyType2D.Static;
                //ConnectHook(targetObj);
                PullPlayer();
                beingShot = false;
                hasHooked = true;
            }
            else if (collision.gameObject.CompareTag("Movable") || collision.gameObject.CompareTag("Assignment") || collision.gameObject.CompareTag("Bomb"))
            {
                audioPlayer.Play();

                if (collision.gameObject.CompareTag("Bomb"))
                    collision.gameObject.GetComponent<Bomb>().startBlowingUp();
                targetObj = collision.gameObject;
                shootDir = Vector3.zero;
                rbHook.velocity = Vector3.zero;
                ConnectHook(targetObj);
                Pull(targetObj);
                beingShot = false;
                hasHooked = true;
                p1.GetComponent<PlayerController>().AddHook(this.gameObject);
            }
        }
    }
    private void ConnectHook(GameObject obj)
    {
        fj.enabled = true;
        fj.frequency = 3;
        fj.connectedBody = obj.GetComponent<Rigidbody2D>();
    }

    private void PullPlayer()
    {
        Debug.Log("Pulling Player is being enabled");
        pulling = true;
        p1.GetComponent<PlayerController>().staticHooksOut += 1;
        currentChain.GetComponent<Chain>().Build(sprite.color);
        if (p1.GetComponent<PlayerController>().staticHooksOut == 1)
        {
            currentChain.GetComponent<Chain>().RetractTo(RetractToLength);
        } else
        {
            currentChain.GetComponent<Chain>().RetractTo(p1.GetComponent<PlayerController>().distBtwnHooks()/2);
        }
    }

    private void Pull(GameObject obj)
    {
        Debug.Log("Pull is being enabled");
        pulling = true;
        currentChain.GetComponent<Chain>().Build(sprite.color);
        currentChain.GetComponent<Chain>().RetractFrom(RetractToLength);
        try
        {
            obj.GetComponent<AssignmentController>().Pulled();
        }
        catch
        {

        }
    }
    

    private void Drop(GameObject obj)
    {
        Debug.Log("Drop: Retracting hook");
        pulling = false;
        fj.connectedBody = null;
        fj.enabled = false;
        try
        {
            obj.GetComponent<AssignmentController>().Dropped();
        }
        catch
        {

        }
    }

    private bool canHook()
    {
        return !hasHooked;
    }
}

/**
public class Hook : MonoBehaviour
{

    private Vector3 shootDir;
    private Rigidbody2D rbHook;
    [SerializeField] private float force;
    [SerializeField] private bool isHook1;
    [SerializeField] private bool beingShot;
    private FixedJoint2D fj;
    private TargetJoint2D tj;
    public float djDist = (float)0.2;
    GameObject p1;

    private bool pulling;
    private GameObject targetObj;
    private bool hasHooked = false;
    [SerializeField]
    private SpriteRenderer sprite;

    private void Start()
    {
        p1 = GameObject.FindGameObjectWithTag("Player");
        rbHook = GetComponent<Rigidbody2D>();
        fj = this.gameObject.AddComponent<FixedJoint2D>();
        fj.enabled = false;

        tj = this.gameObject.AddComponent<TargetJoint2D>();
        tj.frequency = 4;
        tj.enabled = false;

    }

    public void Setup(Vector3 shootDir, bool isArm1)
    {
        Debug.Log("The hook is being shot");
        beingShot = true;
        this.shootDir = shootDir;
        this.isHook1 = isArm1;
        sprite.color = isHook1 ? Color.blue : Color.red;
        //rbHook.velocity = shootDir * force;
    }

    public void Retract()
    {
        if (fj.enabled)
        {
            Drop(targetObj);
        }
        else
        {
            DisconnectRope();
        }
        beingShot = false;
        Destroy(this.gameObject);
    }
    public void ConnectRope()
    {
        
        p1.GetComponent<Grapple>().startGrapple(isHook1, this.gameObject);
    }

    public void DisconnectRope()
    {
        p1.GetComponent<Grapple>().endGrapple(isHook1, this.gameObject);
    }


    /**
     * Bad version of trying to do this
    public void ConnectRope()
    {
        GameObject[] arms = GameObject.FindGameObjectsWithTag("Arm");
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].GetComponent<Rope>().enableRope(isHook1, this.transform);
        }
    }

    public void DisconnectRope()
    {
        GameObject[] arms = GameObject.FindGameObjectsWithTag("Arm");
        for(int i = 0; i < arms.Length; i++)
        {
            arms[i].GetComponent<Rope>().disableRope(isHook1, this.transform);
        }
    }
    **/
/**
    private void FixedUpdate()
    {
        if (beingShot)
        {
            rbHook.velocity = shootDir * force;
        }

        if (pulling)
        {
            tj.target = p1.GetComponent<Transform>().position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (canHook())
        {
            if (collision.gameObject.CompareTag("Platform"))
            {
                ConnectRope();
                shootDir = Vector3.zero;
                rbHook.bodyType = RigidbodyType2D.Static;
                beingShot = false;
                hasHooked = true;
            }
            else if (collision.gameObject.CompareTag("Movable") || collision.gameObject.CompareTag("Assignment"))
            {
                targetObj = collision.gameObject;
                rbHook.velocity = Vector2.zero;
                ConnectHook(targetObj);
                Pull(targetObj);
                hasHooked = true;
            }
        }
    }
    private void ConnectHook(GameObject obj)
    {
        fj.enabled = true;
        fj.frequency = 3;
        fj.connectedBody = obj.GetComponent<Rigidbody2D>();
    }

    private void Pull(GameObject obj)
    {
        Debug.Log("Pull is being enabled");
        pulling = true;

        tj.enabled = true;
        tj.target = p1.GetComponent<Transform>().position;
        try
        {
            obj.GetComponent<AssignmentController>().Pulled();
        } catch
        {

        }
    }

    private void Drop(GameObject obj)
    {
        pulling = false;
        fj.connectedBody = null;
        fj.enabled = false;
        try
        {
            obj.GetComponent<AssignmentController>().Dropped();
        } catch
        {

        }
        
    }

    private bool canHook()
    {
        return !hasHooked;
    }
}
**/
