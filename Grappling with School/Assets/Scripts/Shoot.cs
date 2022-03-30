using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private bool canShoot;
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
        if (canShoot)
        {
            Debug.Log("pewpew");
            ShootHook(sender, e);
        } else
        {
            Debug.Log("Retracting hook");
            Retract(sender, e);
        }
    }

    private void ShootHook(object sender, CharacterAiming.OnShootEventArgs e)
    {
        Vector3 shootDir = (e.aimPoint - e.firePoint.position).normalized;
        hookTransform = Instantiate(pfHook, e.firePoint.position, Quaternion.identity);
        hookTransform.GetComponent<Hook>().Setup(shootDir, e.isArm1);
        canShoot = false;
    }

    private void Retract(object sender, CharacterAiming.OnShootEventArgs e)
    {
        hookTransform.GetComponent<Hook>().Retract();
        canShoot = true;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
