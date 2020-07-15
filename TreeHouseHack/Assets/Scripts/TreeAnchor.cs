using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeHouseHack;

public class TreeAnchor : MonoBehaviour
{
    public float CPTreeOffset = 0;
    public Vector3 CP;  // Update when the tree moves.
    public GameObject Anchor;
    public float elevation;
    public float PlateOffset;
    public GameObject testSphere;

    private void Start()
    {
        if (Anchor != null) { elevation = Anchor.transform.position.y; }            //  Set the Tree Anchor position to that of the tree.  Kinda trick.  Or not.

        testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);              //  DEBUGGING.  Used to show the Connection Point.
        testSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    void LateUpdate()
    {
        CP = Anchor.transform.position + Anchor.transform.forward * CPTreeOffset;           //  On every frame, locate the Connection Point so it's in front of the Tree Anchor.  
        testSphere.transform.position = CP;                                                 //  DEBUGGING.  Get rid of later.
    }
}
