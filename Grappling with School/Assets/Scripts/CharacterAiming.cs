using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAiming : MonoBehaviour
{
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Transform firePoint;
        public Vector3 aimPoint;
        public bool isArm1;
    }

    private GameObject myPlayer;
    [SerializeField] private Transform firePoint;
    private Vector3 aimPoint;
    [SerializeField] private bool isArm1;
    private bool isAiming = true;

    private Vector3 target;

    private void Update()
    {
        if (isArm1)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                OnShoot?.Invoke(this, new OnShootEventArgs { firePoint = firePoint, aimPoint = aimPoint, isArm1 = isArm1 });
            }
        } else
        {
            if (Input.GetButtonDown("Fire2"))
            {
                OnShoot?.Invoke(this, new OnShootEventArgs { firePoint = firePoint, aimPoint = aimPoint, isArm1 = isArm1 });
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
        } else
        {
            Vector3 difference = target - transform.position;
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

    public void Targeting(Transform newTarget)
    {
        isAiming = false;
        target = newTarget.position;
    }

    public void Aiming()
    {
        isAiming = true;
    }
}
