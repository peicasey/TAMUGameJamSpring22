using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private GameObject Player;
    private GameObject Arm1;
    private GameObject Arm2;
    private Rigidbody2D rb;
    private SpringJoint2D sj1;
    private SpringJoint2D sj2;
    public float sjDist = (float)0.1;
    public float sjFreq = (float)0.1;

    // Start is called before the first frame update
    void Start()
    {
        Player = this.gameObject;
        CharacterAiming[] arms = FindObjectsOfType<CharacterAiming>();
        foreach (CharacterAiming arm in arms){
            if (arm.getIsArm1())
            {
                Arm1 = arm.gameObject;
                sj1 = Player.AddComponent<SpringJoint2D>();
                sj1.enabled = false;
            } else
            {
                Arm2 = arm.gameObject;
                sj2 = Player.AddComponent<SpringJoint2D>();
                sj2.enabled = false;
            }
        }
    }

    public void startGrapple(bool isHook1, GameObject hook)
    {
        Debug.Log("SJ is being enabled");
        SpringJoint2D sj;
        if (isHook1)
            sj = sj1;
        else
            sj = sj2;
        sj.enabled = true;
        sj.connectedBody = hook.GetComponent<Rigidbody2D>();
        sj.autoConfigureDistance = false;
        sj.frequency = sjFreq;
        sj.distance = sjDist;
    }

    public void endGrapple(bool isHook1, GameObject hook)
    {
        Debug.Log("SJ is being disabled");
        SpringJoint2D sj;
        if (isHook1)
            sj = sj1;
        else
            sj = sj2;
        sj.connectedBody = null;
        sj.enabled = false;
    }

    private void SetUpSpringJoint(CharacterAiming arm, SpringJoint2D sj)
    {
        // To implement, make it so that the joint connects at the right place
    } 
}
