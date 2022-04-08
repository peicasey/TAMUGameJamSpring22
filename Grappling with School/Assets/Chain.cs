using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public GameObject chainObject;
    private float length;

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

        chainObjectHeight = currentChainLink.GetComponent<Collider2D>().bounds.size.y;
        chainObjectWidth = currentChainLink.GetComponent<Collider2D>().bounds.size.x;

        length = Vector2.Distance(target1.GetComponent<Transform>().position, target2.GetComponent<Transform>().position);

        numOfChains = (int)(length / chainObjectHeight);
        Build();
        ConnectBtwn();
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

    public void ConnectBtwn()
    {
        /**
        FixedJoint2D fj1 = chainLinks[0].AddComponent<FixedJoint2D>();
        FixedJoint2D fj2 = chainLinks[numOfChains - 1].AddComponent<FixedJoint2D>();

        chainLinks[0].transform.position = target1.GetComponent<Transform>().position;
        chainLinks[numOfChains-1].transform.position = target2.GetComponent<Transform>().position;

        fj1.connectedBody = target1.GetComponent<Rigidbody2D>();
        fj2.connectedBody = target2.GetComponent<Rigidbody2D>();
        **/

        FixedJoint2D fj1 = target1.AddComponent<FixedJoint2D>();
        FixedJoint2D fj2 = target2.AddComponent<FixedJoint2D>();

        chainLinks[0].transform.position = target1.GetComponent<Transform>().position;
        chainLinks[numOfChains - 1].transform.position = target2.GetComponent<Transform>().position;

        fj1.connectedBody = chainLinks[0].GetComponent<Rigidbody2D>();
        fj2.connectedBody = chainLinks[numOfChains-1].GetComponent<Rigidbody2D>();
    }
}
