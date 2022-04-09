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

    private GameObject myPlayer;
    [SerializeField] private Transform firePoint;
    private Vector3 aimPoint;
    [SerializeField] private bool isArm1;
    private bool isAiming = true;

    private GameObject target;

    void Start()
    {
        canShoot = 0;
    }

    private void Update()
    {
        if (isArm1)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                onShoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire2"))
            {
                onShoot();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAiming)
        {
            aimPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 difference = aimPoint - transform.position;
            difference.Normalize();
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
        else
        {
            Vector3 difference = target.transform.position - transform.position;
            difference.Normalize();
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
    }

    public bool getIsArm1()
    {
        return isArm1;
    }

    public Transform getFirePoint()
    {
        return firePoint;
    }
    public Vector3 getAimPoint()
    {
        return aimPoint;
    }

    public void Targeting(GameObject newTarget)
    {
        isAiming = false;
        target = newTarget;
    }

    public void Aiming()
    {
        isAiming = true;
    }


    private void onShoot(/**object sender, CharacterAiming.OnShootEventArgs e**/)
    {
        Debug.Log("OnShoot: canshoot: " + canShoot);
        if (canShoot == 0)
        {
            Debug.Log("pewpew");
            ShootHook();
        } else if(!hookTransform)
        {
            ShootHook();
        } else if (canShoot == 1)
        {
            Retract();
        } else if(canShoot == 2)
        {
            Delete();
        } else
        {
            //This shouldn't ever be called but just in case.
            Delete();
            ShootHook();
        }
    }

    private void ShootHook()
    {
        Vector3 shootDir = (aimPoint - firePoint.position).normalized;
        hookTransform = Instantiate(pfHook, firePoint.position, Quaternion.identity);
        hookTransform.GetComponent<Hook>().Setup(this, shootDir, isArm1, firePoint);
        Targeting(hookTransform.gameObject);
        canShoot = 1;
        Debug.Log("ShootHook: canshoot: " + canShoot);
    }


    
    private void Retract()
    {
        Debug.Log("Retract: Retracting hook");
        hookTransform.GetComponent<Hook>().Retract();
        canShoot = 2;
        Debug.Log("Retract: canshoot: " + canShoot);
    }

    private void Delete()
    {
        Debug.Log("Delete: DEleting hook");
        hookTransform.GetComponent<Hook>().Delete();
        canShoot = 0;
        Debug.Log("Retract: canshoot: " + canShoot);
        Aiming();
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
