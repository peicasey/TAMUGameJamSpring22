using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shoot : MonoBehaviour
{
    //0 = no shots fired, can shoot out, 1 = shot fired out, not retracted, 2 = shot fired out, retracted
    //0, the gun fires the hook 1, the hook retracts, 2 the hook deletes itself
    public int canShoot;
    [SerializeField] private Transform pfHook;
    private Transform hookTransform;


    // Start is called before the first frame update
    void Start()
    {
        canShoot = 0;
        CharacterAiming characterAiming = GetComponent<CharacterAiming>();
        characterAiming.OnShoot += CharacterAiming_OnShoot;
    }

    private void CharacterAiming_OnShoot(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Debug.Log("OnShoot: canshoot: " + canShoot);
        if (canShoot == 0)
        {
            Debug.Log("pewpew");
            ShootHook(sender, e);
        } else if(hookTransform == null)
        {
            canShoot = 0;
        } else if (canShoot == 1)
        {
            Retract(sender, e);
        } else if(canShoot == 2)
        {
            Delete(sender, e);
        } else
        {
            //This shouldn't ever be called but just in case.
            Delete(sender, e);
            ShootHook(sender, e);
        }
    }

    private void ShootHook(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Vector3 shootDir = (e.aimPoint - e.firePoint.position).normalized;
        hookTransform = Instantiate(pfHook, e.firePoint.position, Quaternion.identity);
        hookTransform.GetComponent<Hook>().Setup(shootDir, e.isArm1);
        canShoot = 1;
        Debug.Log("ShootHook: canshoot: " + canShoot);
    }


    
    private void Retract(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Debug.Log("Retract: Retracting hook");
        hookTransform.GetComponent<Hook>().Retract();
        canShoot = 2;
        Debug.Log("Retract: canshoot: " + canShoot);
    }

    private void Delete(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Debug.Log("Delete: DEleting hook");
        hookTransform.GetComponent<Hook>().Delete();
        canShoot = 0;
        Debug.Log("Retract: canshoot: " + canShoot);
    }

}

/** Old Hook version
public class Shoot : MonoBehaviour
{
    public bool canShoot;
    [SerializeField] private Transform pfHook;
    private Transform hookTransform;


    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
        CharacterAiming characterAiming = GetComponent<CharacterAiming>();
        characterAiming.OnShoot += CharacterAiming_OnShoot;
    }

    private void CharacterAiming_OnShoot(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Debug.Log("OnShoot: canshoot: " + canShoot);
        if (canShoot)
        {
            Debug.Log("pewpew");
            ShootHook(sender, e);
        } else
        {
            Retract(sender, e);
        }
    }

    private void ShootHook(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Vector3 shootDir = (e.aimPoint - e.firePoint.position).normalized;
        hookTransform = Instantiate(pfHook, e.firePoint.position, Quaternion.identity);
        hookTransform.GetComponent<Hook>().Setup(shootDir, e.isArm1);
        canShoot = false;
        Debug.Log("ShootHook: canshoot: " + canShoot);
    }


    /** Old Hook version
    private void Retract(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Debug.Log("Retract: Retracting hook");
        hookTransform.GetComponent<Hook>().Retract();
        canShoot = true;
        Debug.Log("Retract: canshoot: " + canShoot);
    }

}
**/
