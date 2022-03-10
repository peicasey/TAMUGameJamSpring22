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

    private void Start()
    {
        rbHook = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector3 shootDir, bool isArm1)
    {
        Debug.Log("The hook is being shot");
        beingShot = true;
        this.shootDir = shootDir;
        this.isHook1 = isArm1;
        //rbHook.velocity = shootDir * force;
    }

    public void Retract()
    {
        DisconnectRope();
        Destroy(this.gameObject);
        beingShot = false;
    }
    public void ConnectRope()
    {
        GameObject p1 = GameObject.FindGameObjectWithTag("Player");
        p1.GetComponent<Grapple>().startGrapple(isHook1, this.gameObject);
    }

    public void DisconnectRope()
    {
        GameObject p1 = GameObject.FindGameObjectWithTag("Player");
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            ConnectRope();
            shootDir = Vector3.zero;
            rbHook.bodyType = RigidbodyType2D.Static;
            beingShot = false;
        } 
    }
}
