using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeHouseHack
{
    public class Deck : MonoBehaviour
    {
        private Treehouse th = new Treehouse();

        public List<Plate> Plates = new List<Plate>();   //  Make private after testing.
        public int BeamPlateOverlap = 0;            // Amount  the deck overlaps beams, typically.
        public GameObject PlateType;                // GameObject of the deck plates.
        public float PlateWidth = .1f;              // Width of the deck plates.
        private Bounds PlateBounds;                      // This is the extents of the plate.
        private float DatumDistance = 0;            // Longest span.
        public int ActivePlates = 0;                // Plates currently on the deck.
        public Vector3 ProjectionPoint;             // The orthogonal projection point, in world space.
        public Vector3 test1;                       // DEBUGGING
        public Vector3 test2;                       // DEBUGGING
        public Vector3 test3;                       // DEBUGGING
        public GameObject testsphere;               // DEBUGGING
        public Vector3 GenerationDirection;         //  Change to private after DEBUG>
        public Vector3 Orientation;                 //  Change to private after DEBUG>

        private void Awake()
        {
            th = gameObject.GetComponent<Treehouse>();

            if (PlateType == null)  PlateType = Resources.Load("Prefabs/Plate") as GameObject;          //  If no plate loaded, do a 2x4.

            PlateWidth = PlateType.GetComponent<MeshRenderer>().bounds.size.x;                          //  Set the size, bounds of the Plate.
            PlateBounds = PlateType.GetComponent<MeshRenderer>().bounds;

            testsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);      /// DEBUGGING
        }

        public void ManagePlates()
        {
            if (th.Trees.Count < 3)         {Debug.Log("Not enough trees.");  return;        }

            UpdateDatums();

            if (this.Plates.Count == 0)
            {
                for (int i = 0; i < (DatumDistance / PlateWidth); i++)
                {
                    AddPlate();
                }
                ActivePlates = this.Plates.Count;
            }
            else
            {
                int delta = Mathf.FloorToInt(DatumDistance / PlateWidth) - ActivePlates;

                if (delta < 0)
                {
                    for (int j = 0; j > delta; j--)             // Lose Plates.
                    {
                        Plates[Plates.Count - 1 + j].transform.position = new Vector3(0, -10f, 0);
                    }
                }
                else if (delta > 0)
                {
                    for (int j = 0; j < delta; j++)           //  Add Plates.
                    {
                        AddPlate();
                    }
                }
                ActivePlates = ActivePlates + delta; 
            }

            UpdateOrientation();

            void AddPlate()
            {
                GameObject NewPlate = Instantiate(PlateType);
                NewPlate.transform.parent = this.transform;
                Plates.Add(NewPlate.GetComponent<Plate>());
            }
        }

        private void UpdateOrientation()
        {
            List<GameObject> OT = new List<GameObject>();
            OT = transform.GetComponent<Treehouse>().OrderedTrees;

            th = gameObject.GetComponent<Treehouse>();          //  Doing this to verify I don't have a weird ref problem.

            if (DatumDistance != 0) ProjectionPoint = GetAlttitudePoint();

            for (int i = 0; i < Plates.Count; i++)
            {
                int length = 16;

                GenerationDirection = OT[1].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP;
                GenerationDirection = OT[1].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP;
                GenerationDirection.Normalize();

                Orientation = OT[2].GetComponent<TreeAnchor>().CP - ProjectionPoint;
                Orientation.Normalize();

                if (i < ActivePlates)
                {
                    Plates[i].transform.forward = Orientation;
                    Plates[i].transform.position = OT[0].GetComponent<TreeAnchor>().CP + PlateBounds.size.x * GenerationDirection * i;
                }
                else
                {
                    Plates[i].transform.position = new Vector3(0, -50, 0);
                }

                //Plates[i].transform.position = this.transform.position + this.transform.rotation * new Vector3(Offset.size.x * i + th.Trees[0].GetComponent<Tree>().Diameter, 0, -Offset.size.z / 16f * BeamPlateOverlap);
                //Plates[i].transform.localScale = new Vector3(1f, 1f, length / 16f);
            }
        }

        public Vector3 GetAlttitudePoint()
        {
            List<GameObject> OT = new List<GameObject>();
            OT = transform.GetComponent<Treehouse>().OrderedTrees;

            test1 = OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().CP - OT[1].GetComponent<Tree>().GetComponent<TreeAnchor>().CP;
            test2 = OT[2].GetComponent<Tree>().GetComponent<TreeAnchor>().CP - OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().CP;

            test1.Normalize();
            test2.Normalize();

            float cos = Mathf.Cos(Vector3.Angle(test1, test2) * Mathf.Deg2Rad);

            //float cos = Mathf.Cos(Vector3.Angle(th.Trees[1].GetComponent<TreeAnchor>().CP - th.Trees[0].GetComponent<TreeAnchor>().CP, th.Trees[2].GetComponent<TreeAnchor>().CP - th.Trees[0].GetComponent<TreeAnchor>().CP));

            //  Use the Leg Rule (proportion).  https://mathbitsnotebook.com/Geometry/RightTriangles/RTmeanRight.html
            float hypotenuse = Vector3.Distance(OT[0].GetComponent<TreeAnchor>().CP, OT[1].GetComponent<TreeAnchor>().CP);
            float projection = Mathf.Pow(Vector3.Distance(OT[2].GetComponent<TreeAnchor>().CP, OT[0].GetComponent<TreeAnchor>().CP),2) / hypotenuse ;       // This is wrong.

            cos = cos * Vector3.Distance(OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().CP, OT[2].GetComponent<Tree>().GetComponent<TreeAnchor>().CP);

            Debug.Log("h: " + hypotenuse.ToString() + "p: " + projection.ToString() + " cos: "  + cos.ToString());

            Vector3 direction = (OT[1].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP);
            direction.Normalize();
            //Vector3 altPoint = OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().CP + direction * Mathf.Abs(projection);

            //testsphere.transform.position = altPoint;
            Vector3 altPoint = OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().CP + direction * Mathf.Abs(cos);

            testsphere.transform.position = OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().CP + direction * Mathf.Abs(cos);
            
            return altPoint;
        }

        public void UpdateDatums()
        {
            List<GameObject> OT = new List<GameObject>();
            OT = transform.GetComponent<Treehouse>().OrderedTrees;

            DatumDistance = Vector3.Distance(OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().CP, OT[1].GetComponent<Tree>().GetComponent<TreeAnchor>().CP) - OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().PlateOffset - OT[0].GetComponent<Tree>().GetComponent<TreeAnchor>().PlateOffset;
        }
    }
}

