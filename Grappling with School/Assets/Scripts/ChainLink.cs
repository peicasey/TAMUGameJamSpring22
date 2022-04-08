using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLink : MonoBehaviour
{
    public Transform top;
    public Transform bottom;
    public GameObject nextPos;
    public float height;
    public float width;

    public Collider2D cd;

    private void Start()
    {
        cd = this.GetComponent<Collider2D>();
        height = cd.bounds.size.x;
        width = cd.bounds.size.y;
    }

}
