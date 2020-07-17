using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TreeHouseHack;

public class Treehouse : MonoBehaviour
{
    public List<GameObject> Trees = new List<GameObject>(3);
    public List<GameObject> OrderedTrees = new List<GameObject>(3);
    public bool NeedRefresh = false;
    public float Elevation = 2f;
    public float RailElevation = 1f;

    public GameObject testSphere;

    private void Start()
    {
        foreach (var t in Trees)
        {
            TreeAnchor ta = t.gameObject.AddComponent<TreeAnchor>();
            GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ta.Anchor = anchor;
            anchor.transform.position = t.transform.position + new Vector3(0f, Elevation, 0f);

            ta.CPTreeOffset = 0.5f;
            ta.elevation = Elevation;
            ta.PlateOffset  = 0f;
            ta.diameter = 0.5f;
            anchor.transform.localScale = new Vector3(ta.diameter, ta.diameter, ta.diameter);
            ta.Anchor.layer = 8;

        }
        UpdateAnchors();
    }

    void Update()   {
        foreach (var t in Trees)    {
            GameObject anchor = t.GetComponent<TreeAnchor>().Anchor;
            if (anchor != null && !NeedRefresh) { if (anchor.transform.hasChanged) { NeedRefresh = true; } }            //  If we don't need to refresh, check if the anchor has changed.
    }   }

    private void LateUpdate()
    {
        if (NeedRefresh) UpdateAnchors();               //  After all the action, check if we need to refresh our stuff.
    }

    public void OrderTrees()            // This method orders the trees so we know triangle vertices.
    {
        OrderedTrees.Clear();

        float b1 = 0;
        float b2 = 0;
        float b3 = 0;

        GameObject t0 = Trees[0];
        GameObject t1 = Trees[1];
        GameObject t2 = Trees[2];
        GameObject tt;

        b1 = Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP);
        b2 = Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP); 
        b3 = Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP); 

        //  b1 is longest.  b3 is shortest.
        if (b1 < b2)  //  b1 is the longest.
        {
            tt = t0; t0 = t1; t1 = tt;
        }

        b1 = Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP);
        b2 = Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP);
        b3 = Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP);

        if (b1 < b3)
        {
            tt = t0; t0 = t2; t2 = tt;
        }

        b1 = Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t0.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP);
        b2 = Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t2.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t1.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP);
        b3 = Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t0.GetComponent<TreeAnchor>().CP) + Vector3.Distance(t2.GetComponent<TreeAnchor>().CP, t1.GetComponent<TreeAnchor>().CP);

        if (b2 < b3)
        {
            tt = t1; t1 = t2; t2 = tt;
        }

        OrderedTrees.Add(t0);
        OrderedTrees.Add(t1);
        OrderedTrees.Add(t2);

        // DEBUGGING
        OrderedTrees[0].GetComponent<TreeAnchor>().Anchor.GetComponent<MeshRenderer>().material.color = Color.red;
        OrderedTrees[1].GetComponent<TreeAnchor>().Anchor.GetComponent<MeshRenderer>().material.color = Color.green;
        OrderedTrees[2].GetComponent<TreeAnchor>().Anchor.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void UpdateAnchors()         //  Called only when we need to regenerate the Tree House.
    {
        NeedRefresh = false;            //  Reset our flag.
        OrderTrees();                   //  Figure out my trees.

        foreach (var t in Trees)        //  We're assuming all tree anchors need to change.  (Good assumption.)
        {
            GameObject anchor = t.GetComponent<TreeAnchor>().Anchor;

            if (anchor != null)             
            {
                t.transform.position = anchor.transform.position - new Vector3(0, t.GetComponent<TreeAnchor>().elevation, 0);       //  If we've moved the anchor, move the tree.  Account for elevation.

                List<GameObject> otherTrees = new List<GameObject>();                       //  Get a list of other trees.  
                foreach (var o in Trees) { if (o != t) { otherTrees.Add(o); } }

                if (otherTrees.Count == 1)                  //  If there are other trees.
                {
                    anchor.transform.LookAt(otherTrees[0].GetComponent<TreeAnchor>().Anchor.transform);         //  If there's only one tree, look at the other tree.  SIMPLE!
                }
                else if (otherTrees.Count == 2)
                {
                    anchor.transform.LookAt((otherTrees[0].GetComponent<TreeAnchor>().Anchor.transform.position + otherTrees[1].GetComponent<TreeAnchor>().Anchor.transform.position) / 2);  // If two trees, split.
                }
                t.GetComponent<TreeAnchor>().CP = anchor.transform.position + anchor.transform.forward * t.GetComponent<TreeAnchor>().CPTreeOffset;         // Change the Connection Point of the Tree Anchor.
            }
        }
       gameObject.GetComponent<Deck>().DrawDeck();
    }
}