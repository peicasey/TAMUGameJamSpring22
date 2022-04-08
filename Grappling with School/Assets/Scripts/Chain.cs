using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public GameObject chainObject;
    [SerializeField]
    private float length;

    public GameObject target1;
    public GameObject target2;

    private float chainObjectWidth;
    private float chainObjectHeight;
    [SerializeField]
    private List<GameObject> chainLinks = new List<GameObject> ();
    [SerializeField]
    private int numOfChains;
    private GameObject currentChainLink;
    private GameObject previousChainLink;

    FixedJoint2D fj1;
    FixedJoint2D fj2;

    HingeJoint2D hj1;
    HingeJoint2D hj2;

    ///**Testing val
    public float retractToLen;
    //**/


    private void Start()
    {

    }

    private void Update()
    {
    }

    public void Build()
    {
        //I have no idea why but it was spawning the list with 2 null gameObjects. I did this as a temp fix
        chainLinks.Clear();
        numOfChains = 0;


        currentChainLink = Instantiate<GameObject>(chainObject, this.transform);
        chainLinks.Add(currentChainLink);
        chainLinks[0].name = "Chain 0";
        //Debug.Log(chainLinks);
        if (!(currentChainLink))
        {
            Debug.LogError("Fix Chain Script Null reference");
        }

        chainObjectHeight = currentChainLink.GetComponent<Collider2D>().bounds.size.y;
        chainObjectWidth = currentChainLink.GetComponent<Collider2D>().bounds.size.x;

        length = Vector2.Distance(target1.GetComponent<Transform>().position, target2.GetComponent<Transform>().position);
        //Debug.Log("Length: " + length);

        numOfChains = (int)(length / chainObjectHeight);
        Spawn();
        ConnectBtwn();
    }

    public void Spawn()
    {
        //Fixes janky bug where the object is forced to a new position
        chainLinks[0].transform.position = target1.GetComponent<Transform>().position;
        Debug.Log("Building " + numOfChains + "chains");
        for(int i = 1; i < numOfChains; i++)
        {
            //Spawning and adding to list
            currentChainLink = Instantiate<GameObject>(chainObject, this.transform);
            chainLinks.Add(currentChainLink);
            chainLinks[i].name = "Chain " + i;
            //Debug.Log(chainLinks);
            chainLinks[i].GetComponent<Transform>().position = chainLinks[i - 1].GetComponent<ChainLink>().nextPos.transform.position;

            //Adding HingeJoint
            HingeJoint2D currentHJ = chainLinks[i].AddComponent<HingeJoint2D>();
            currentHJ.autoConfigureConnectedAnchor = false;
            currentHJ.connectedBody = chainLinks[i-1].GetComponent<Rigidbody2D>();

            //Debug.Log("Connected Anchor: " + chainLinks[i].GetComponent<ChainLink>().top.localPosition);
            //Debug.Log(" Anchor: " + chainLinks[i - 1].GetComponent<ChainLink>().bottom.localPosition);

            currentHJ.connectedAnchor = chainLinks[i].GetComponent<ChainLink>().top.localPosition;
            currentHJ.anchor = chainLinks[i-1].GetComponent<ChainLink>().bottom.localPosition;

            //Trying to fix janky spawning bug
            chainLinks[i].transform.position = target2.GetComponent<Transform>().position;

            //Trying to fix launching across the map bug
            chainLinks[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        //Fixes janky bug where the object is forced to a new position
        chainLinks[numOfChains - 1].transform.position = target2.GetComponent<Transform>().position;
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

        /**
        fj1 = target1.AddComponent<FixedJoint2D>();
        fj2 = target2.AddComponent<FixedJoint2D>();

        chainLinks[0].transform.position = target1.GetComponent<Transform>().position;
        chainLinks[numOfChains - 1].transform.position = target2.GetComponent<Transform>().position;

        fj1.connectedBody = chainLinks[0].GetComponent<Rigidbody2D>();

        fj2.connectedBody = chainLinks[numOfChains-1].GetComponent<Rigidbody2D>();

        fj1.autoConfigureConnectedAnchor = false;
        fj1.connectedAnchor = chainLinks[0].GetComponent<ChainLink>().bottom.localPosition;
        fj2.autoConfigureConnectedAnchor = false;
        fj2.connectedAnchor = chainLinks[numOfChains - 1].GetComponent<ChainLink>().top.localPosition;
        **/
        hj1 = target1.AddComponent<HingeJoint2D>();
        hj2 = target2.AddComponent<HingeJoint2D>();

        chainLinks[0].transform.position = target1.GetComponent<Transform>().position;
        chainLinks[numOfChains - 1].transform.position = target2.GetComponent<Transform>().position;

        hj1.connectedBody = chainLinks[0].GetComponent<Rigidbody2D>();
        hj2.connectedBody = chainLinks[numOfChains-1].GetComponent<Rigidbody2D>();

        hj1.autoConfigureConnectedAnchor = false;
        hj1.connectedAnchor = chainLinks[0].GetComponent<ChainLink>().bottom.localPosition;
        hj2.autoConfigureConnectedAnchor = false;
        hj2.connectedAnchor = chainLinks[numOfChains - 1].GetComponent<ChainLink>().top.localPosition;

    }

    public void Retract()
    {
        //Debug.Log("Retracting");
        Destroy(chainLinks[numOfChains - 1]);
        chainLinks.RemoveAt(numOfChains - 1);
        numOfChains--;
        hj2.connectedBody = chainLinks[numOfChains - 1].GetComponent<Rigidbody2D>();
        //Debug.Log("Done waiting");
    }

    public void RetractTo(float targetLength)
    {
        int targetNumOfChains = Mathf.CeilToInt(targetLength / chainObjectHeight);
        Debug.Log("Retracting to" + targetNumOfChains);
        for(int i = numOfChains-1; i > targetNumOfChains; i--) {
            Debug.Log(i);
            if (i <= 0)
                Destroy(this.transform);
            //Debug.Log("Retracting");
            Destroy(chainLinks[i]);
            chainLinks.RemoveAt(i);
            hj2.connectedBody = chainLinks[i - 1].GetComponent<Rigidbody2D>();
        }
        numOfChains = chainLinks.Count;
        Debug.Log("Retracting complete");
    }
}
