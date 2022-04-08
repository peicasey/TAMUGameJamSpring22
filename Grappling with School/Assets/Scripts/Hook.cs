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
