using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TreeHouseHack;


public class Treehouse : MonoBehaviour
{
    public List<GameObject> Trees = new List<GameObject>(3);
    public bool NeedRefresh = true;
       
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var t in Trees)
        {
            GameObject anchor = t.GetComponent<TreeAnchor>().Anchor;
            if (anchor != null && !NeedRefresh) { if (anchor.transform.hasChanged) { NeedRefresh = true; } }
        }
    }

    private void LateUpdate()
    {
        if (NeedRefresh) UpdateAnchors();
    }

    public void OrderTrees()
    {
        float b1 = 0;
        float b2 = 0;
        float b3 = 0;

        GameObject t0 = Trees[0];
        GameObject t1 = Trees[1];
        GameObject t2 = Trees[2];
        GameObject tt;
        
        b1 = Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP);
        b2 = Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP);
        b3 = Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP);

        //  b1 is longest.  b2 is shortest.
        if (b1 < b2)  //  b1 is the longest.
        {
            tt = t0; t0 = t1; t1 = tt;
        }

        b1 = Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP);
        b2 = Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP);
        b3 = Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP);

        if (b1 < b3)
        {
            tt = t0; t0 = t2; t2 = tt;
        }

        b1 = Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP);
        b2 = Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP);
        b3 = Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP);

        if (b2 < b3)
        {
            tt = t1; t1 = t2; t2 = tt;
        }

        Trees[0] = t0;
        Trees[1] = t1;
        Trees[2] = t2;

        Trees[0].GetComponent<TreeAnchor>().Anchor.GetComponent<MeshRenderer>().material.color = Color.red; 
        Trees[1].GetComponent<TreeAnchor>().Anchor.GetComponent<MeshRenderer>().material.color = Color.green;
        Trees[2].GetComponent<TreeAnchor>().Anchor.GetComponent<MeshRenderer>().material.color = Color.blue;

    }

    public void UpdateAnchors()
    {
        OrderTrees();
        NeedRefresh = false;

        foreach (var t in Trees)
        {
            GameObject anchor = t.GetComponent<TreeAnchor>().Anchor;

            if (anchor != null)
            {
                t.transform.position = anchor.transform.position - new Vector3(0, t.GetComponent<TreeAnchor>().elevation, 0);

                List<GameObject> otherTrees = new List<GameObject>();

                foreach (var o in Trees) { if (o != t) { otherTrees.Add(o); } }

                if (otherTrees.Count == 1)
                {
                    anchor.transform.LookAt(otherTrees[0].GetComponent<TreeAnchor>().Anchor.transform);
                }
                else if (otherTrees.Count == 2)
                {
                    anchor.transform.LookAt((otherTrees[0].GetComponent<TreeAnchor>().Anchor.transform.position + otherTrees[1].GetComponent<TreeAnchor>().Anchor.transform.position) / 2);
                }
                t.GetComponent<TreeAnchor>().CP = anchor.transform.position + anchor.transform.forward * t.GetComponent<TreeAnchor>().CPTreeOffset;
            }

        }
       gameObject.GetComponent<Deck>().ManagePlates();
    }
}
