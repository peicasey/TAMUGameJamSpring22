using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public GameObject chainObject;
    public float length;

    public Rigidbody2D anchoredBody;
    public Rigidbody2D connectedBody;

    public GameObject target1;
    public GameObject target2;

    private float chainObjectWidth;
    private float chainObjectHeight;
    [SerializeField]
    private List<GameObject> chainLinks = new List<GameObject> ();
    private int numOfChains;
    private GameObject currentChainLink;
    private GameObject previousChainLink;


    private void Start()
    {
        currentChainLink = Instantiate<GameObject>(chainObject, this.transform);
        chainLinks.Add(currentChainLink);
        if (!(currentChainLink))
        {
            Debug.LogError("Fix Chain Script Null reference");
        }

        chainObjectHeight = currentChainLink.GetComponent<Collider2D>().bounds.size.x;
        chainObjectWidth = currentChainLink.GetComponent<Collider2D>().bounds.size.x;

        numOfChains = (int)(length / chainObjectHeight);
        Build();
    }

    public void Build()
    {
        Spawn();
    }

    public void Spawn()
    {
        Debug.Log("Building " + numOfChains + "chains");
        for(int i = 1; i < numOfChains; i++)
        {
            //Spawning and adding to list
            currentChainLink = Instantiate<GameObject>(chainObject, this.transform);
            chainLinks.Add(currentChainLink);
            chainLinks[i].GetComponent<Transform>().position = chainLinks[i - 1].GetComponent<ChainLink>().nextPos.transform.position;

            //Adding HingeJoint
            HingeJoint2D currentHJ = chainLinks[i].AddComponent<HingeJoint2D>();
            currentHJ.autoConfigureConnectedAnchor = false;
            currentHJ.connectedBody = chainLinks[i-1].GetComponent<Rigidbody2D>();
            currentHJ.connectedAnchor = chainLinks[i].GetComponent<ChainLink>().top.localPosition;
            currentHJ.anchor = chainLinks[i-1].GetComponent<ChainLink>().bottom.localPosition;
        }

    }

    public void ConnectBtwn(GameObject target1, GameObject target2)
    {
        //target1.get
    }
}
