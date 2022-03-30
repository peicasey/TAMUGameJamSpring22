using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Distance joint version of script
public class Grapple : MonoBehaviour
{
    private GameObject Player;
    private GameObject Arm1;
    private GameObject Arm2;
    private GameObject Hook1;
    private GameObject Hook2;
    private Rigidbody2D rb;
    private DistanceJoint2D dj1;
    private DistanceJoint2D dj2;
    public float djDist = (float)0.5;

    // Start is called before the first frame update
    void Start()
    {
        Player = this.gameObject;
        CharacterAiming[] arms = FindObjectsOfType<CharacterAiming>();
        foreach (CharacterAiming arm in arms){
            if (arm.getIsArm1())
            {
                Arm1 = arm.gameObject;
                dj1 = Player.AddComponent<DistanceJoint2D>();
                SetUpJoint(Arm1, dj1);
            } else
            {
                Arm2 = arm.gameObject;
                dj2 = Player.AddComponent<DistanceJoint2D>();
                SetUpJoint(Arm2, dj2);
            }
        }
    }

    public void startGrapple(bool isHook1, GameObject hook)
    {
        Debug.Log("Joint is being enabled");
        DistanceJoint2D dj;
        if (isHook1) {
            dj = dj1;
            Hook1 = hook;
        }
        else {
            dj = dj2;
            Hook2 = hook;
        }

        dj.enabled = true;
        dj.connectedBody = hook.GetComponent<Rigidbody2D>();
        dj.distance = djDist;
        if (dj1.enabled && dj2.enabled)
        {
            float distanceBtwnPts = Vector3.Distance(Hook1.GetComponent<Transform>().position, Hook2.GetComponent<Transform>().position);
            dj1.distance = distanceBtwnPts / 2;
            dj2.distance = distanceBtwnPts / 2;
            Debug.Log(distanceBtwnPts);
        }
    }

    public void endGrapple(bool isHook1, GameObject hook)
    {
        Debug.Log("Joint is being disabled");
        DistanceJoint2D dj;
        if (isHook1)
        {
            dj = dj1;
        }
        else
        {
            dj = dj2;
        }
        dj.connectedBody = null;
        dj.enabled = false;
        dj1.distance = djDist;
        dj2.distance = djDist;

    }

    private void SetUpJoint(GameObject arm, DistanceJoint2D dj)
    {
        dj.enabled = false;
        dj.maxDistanceOnly = true;
        // To implement, make it so that the joint connects at the right place
    } 

}
// Spring joint version of script
/**
public class Grapple : MonoBehaviour
{
    private GameObject Player;
    private GameObject Arm1;
    private GameObject Arm2;
    private GameObject Hook1;
    private GameObject Hook2;
    private Rigidbody2D rb;
    //private SpringJoint2D sj1;
    //private SpringJoint2D sj2;
    private DistanceJoint2D dj1;
    private DistanceJoint2D dj2;
    //public float sjDist = (float)0.1;
    public float djDist = (float)0.1;
    //public float sjFreq = (float)0.1;

    // Start is called before the first frame update
    void Start()
    {
        Player = this.gameObject;
        CharacterAiming[] arms = FindObjectsOfType<CharacterAiming>();
        foreach (CharacterAiming arm in arms)
        {
            if (arm.getIsArm1())
            {
                Arm1 = arm.gameObject;
                sj1 = Player.AddComponent<SpringJoint2D>();
                sj1.enabled = false;
            }
            else
            {
                Arm2 = arm.gameObject;
                sj2 = Player.AddComponent<SpringJoint2D>();
                sj2.enabled = false;
            }
        }
    }

    public void startGrapple(bool isHook1, GameObject hook)
    {
        Debug.Log("Joint is being enabled");
        SpringJoint2D sj;
        if (isHook1)
        {
            sj = sj1;
            Hook1 = hook;
        }
        else
        {
            sj = sj2;
            Hook2 = hook;
        }
        sj.enabled = true;
        sj.connectedBody = hook.GetComponent<Rigidbody2D>();
        sj.autoConfigureDistance = false;
        sj.frequency = sjFreq;
        sj.distance = sjDist;
        if(sj1.enabled && sj2.enabled)
        {
            float distanceBtwnPts = Vector3.Distance(Hook1.GetComponent<Transform>().position, Hook2.GetComponent<Transform>().position);
            sj1.distance = distanceBtwnPts / 2;
            sj2.distance = distanceBtwnPts / 2;
            Debug.Log(distanceBtwnPts);
        }

    }

    public void endGrapple(bool isHook1, GameObject hook)
    {
        Debug.Log("Joint is being disabled");

        SpringJoint2D sj;
        if (isHook1)
        {
            sj = sj1;
        }
        else
        {
            sj = sj2;
        }
        sj.connectedBody = null;
        sj.enabled = false;
        sj1.distance = sjDist;
        sj2.distance = sjDist;
    }

    private void SetUpJoint(CharacterAiming arm, SpringJoint2D sj)
    {
        // To implement, make it so that the joint connects at the right place
    }
}
**/

