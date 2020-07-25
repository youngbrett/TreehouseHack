using UnityEngine;

public class TreeAnchor : MonoBehaviour
{
    public float CPTreeOffset = 0;
    public float PlateOffset;
    public float elevation;
    public float diameter;

    public Vector3 CP;  // Update when the tree moves.
    public Vector3 CP1;  // Update when the tree moves.
    public Vector3 CP2;  // Update when the tree moves.

    public GameObject Anchor;
    public GameObject testSphere;

    private void Start()
    {
        if (Anchor != null) { elevation = Anchor.transform.position.y; }            //  Set the Tree Anchor position to that of the tree.  Kinda trick.  Or not.

        testSphere = GameObject.CreatePrimitive(PrimitiveType.Cylinder);              //  DEBUGGING.  Used to show the Connection Point.
        testSphere.transform.localScale = new Vector3(0.4f, 0.05f, 0.4f);
    }

    void LateUpdate()
    {
        //CP = Anchor.transform.position - Anchor.transform.forward * CPTreeOffset;           //  On every frame, locate the Connection Point so it's in front of the Tree Anchor.  
        CP = Anchor.transform.position;
        testSphere.transform.position = CP;                                                 //  DEBUGGING.  Get rid of later.
    }
}
