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
        if (Anchor != null)
        {
            elevation = Anchor.transform.position.y;
        }

        testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    void Update()
    {
        CP = Anchor.transform.position + Anchor.transform.forward * CPTreeOffset;
        testSphere.transform.position = CP;
    }

}
