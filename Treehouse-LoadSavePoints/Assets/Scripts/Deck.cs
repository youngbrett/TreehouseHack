using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeHouseHack
{
    public class Deck : MonoBehaviour
    {
        private Treehouse th = new Treehouse();

        private List<Plate> Plates = new List<Plate>();   //  Make private after testing.
        private List<Beam> Beams = new List<Beam>();   //  Make private after testing.
        private List<Rail> Rails = new List<Rail>();   //  Make private after testing.

        public int BeamPlateOverlap = 0;            // Amount  the deck overlaps beams, typically.
        public GameObject PlateType;                // GameObject of the deck plates.
        public GameObject BeamType;                // GameObject of the deck beams.
        public GameObject RailType;                // GameObject of the deck beams.

        private float PlateWidth = .1f;              // Width of the deck plates.
        private Bounds PlateBounds;                      // This is the extents of the plate.
        private float DatumDistance = 0;            // Longest span.
        private int ActivePlates = 0;                // Plates currently on the deck.
        private int ActiveBeams = 0;                // Plates currently on the deck.
        private int ActiveRails = 0;

        private Vector3 ProjectionPoint;             // The orthogonal projection point, in world space.
        private Vector3 test1;                       // DEBUGGING
        private Vector3 test2;                       // DEBUGGING
        private Vector3 test3;                       // DEBUGGING
        private GameObject testsphere;               // DEBUGGING
        private Vector3 GenerationDirection;         //  Change to private after DEBUG>
        private Vector3 Orientation;                 //  Change to private after DEBUG>
        private float PrimaryAngle;
        private float SecondaryAngle;

        private void Awake()
        {
            th = gameObject.GetComponent<Treehouse>();

            if (PlateType == null) PlateType = Resources.Load("Prefabs/Plate") as GameObject;          //  If no plate loaded, do a 2x4.
            if (BeamType == null) BeamType = Resources.Load("Prefabs/Beam") as GameObject;
            if (RailType == null) RailType = Resources.Load("Prefabs/Rail") as GameObject;

            PlateWidth = PlateType.GetComponent<MeshRenderer>().bounds.size.x;                          //  Set the size, bounds of the Plate.
            PlateBounds = PlateType.GetComponent<MeshRenderer>().bounds;



         //   testsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);      /// DEBUGGING
        }

        public void DrawDeck()
        {
            if (th.Trees.Count < 3) { Debug.Log("Not enough trees."); return; }

            UpdateDatums();
            ManagePlates();
            ManageBeams();
            ManageRails();
        }

        public void ManageBeams()
        {
            if (this.Beams.Count <3 )
            {
                int total = this.Beams.Count;

                for (int i = 0; i < (3 - total); i++)
                {
                    AddBeam();
                }
                ActiveBeams = this.Beams.Count;
            }

        PlaceBeam(Beams[0], th.OrderedTrees[0].GetComponent<TreeAnchor>().CP, th.OrderedTrees[1].GetComponent<TreeAnchor>().CP);
        PlaceBeam(Beams[1], th.OrderedTrees[0].GetComponent<TreeAnchor>().CP, th.OrderedTrees[2].GetComponent<TreeAnchor>().CP);
        PlaceBeam(Beams[2], th.OrderedTrees[2].GetComponent<TreeAnchor>().CP, th.OrderedTrees[1].GetComponent<TreeAnchor>().CP);

        void AddBeam()
        {
            GameObject NewBeam = Instantiate(BeamType);
            NewBeam.transform.parent = this.transform;
            Beams.Add(NewBeam.GetComponent<Beam>());
        }

        void PlaceBeam(Beam beam, Vector3 start, Vector3 end)
        {
            beam.transform.position = start;
            beam.transform.rotation = Quaternion.LookRotation(end - start);
            beam.transform.localScale = new Vector3(1, 1, Vector3.Distance(end, start) / 2.4384f);
        }
        }

        public void ManageRails()
        {
            if (this.Rails.Count < 3)
            {
                int total = this.Rails.Count;
                for (int i = 0; i < (3 - total); i++)
                {
                    AddRail();
                }
                ActiveRails = this.Rails.Count;
            }

        PlaceRail(Rails[0], th.OrderedTrees[0].GetComponent<TreeAnchor>().CP, th.OrderedTrees[1].GetComponent<TreeAnchor>().CP);
        PlaceRail(Rails[1], th.OrderedTrees[0].GetComponent<TreeAnchor>().CP, th.OrderedTrees[2].GetComponent<TreeAnchor>().CP);
        PlaceRail(Rails[2], th.OrderedTrees[2].GetComponent<TreeAnchor>().CP, th.OrderedTrees[1].GetComponent<TreeAnchor>().CP);

        void AddRail()
        {
            GameObject NewRail = Instantiate(RailType);
            NewRail.transform.parent = this.transform;
            Rails.Add(NewRail.GetComponent<Rail>());
        }

        void PlaceRail(Rail rail, Vector3 start, Vector3 end)
        {
            rail.transform.position = start + new Vector3(0, th.RailElevation, 0);
            rail.transform.rotation = Quaternion.LookRotation(end - start, Vector3.up);
            rail.transform.localScale = new Vector3(1, 1, Vector3.Distance(end, start) / 2.4384f);
            rail.RailLength = Vector3.Distance(end, start);
            }

        }

        public void ManagePlates()
        {      
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
            UpdateDatums();

            List<GameObject> OT = new List<GameObject>();
            OT = transform.GetComponent<Treehouse>().OrderedTrees;
            if (DatumDistance != 0) ProjectionPoint = GetAlttitudePoint();
            int peak = Mathf.FloorToInt(Vector3.Distance(ProjectionPoint, OT[0].GetComponent<TreeAnchor>().CP) / PlateWidth);

            for (int i = 0; i < Plates.Count; i++)
            {
                GenerationDirection = OT[1].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP;
                GenerationDirection.Normalize();

                Orientation = OT[2].GetComponent<TreeAnchor>().CP - ProjectionPoint;
                Orientation.Normalize();

                if (i < ActivePlates)
                {
                    Plates[i].transform.forward = Orientation;
                    Plates[i].transform.position = OT[0].GetComponent<TreeAnchor>().CP + PlateBounds.size.x * GenerationDirection * i;

                    if (i <= peak)
                    {
                        Plates[i].transform.localScale = new Vector3(1, 1, Mathf.Abs(Mathf.Tan((PrimaryAngle) * Mathf.Deg2Rad) * (i + 1) * PlateWidth / 2.4384f));
                    }
                    else
                    {
                        Plates[i].transform.localScale = new Vector3(1, 1, Plates[peak].transform.localScale.z - Mathf.Abs(Mathf.Tan((SecondaryAngle) * Mathf.Deg2Rad) * (peak - i + 1) * PlateWidth / 2.4384f)); /// (i - (peak / ActivePlates) * i) * );
                    }
                }
                else
                {
                    Plates[i].transform.position = new Vector3(0, -50, 0);
                }
            }
        }

        public Vector3 GetAlttitudePoint()
        {
            List<GameObject> OT = new List<GameObject>();
            OT = transform.GetComponent<Treehouse>().OrderedTrees;

            test1 = OT[0].GetComponent<TreeAnchor>().CP - OT[1].GetComponent<TreeAnchor>().CP;
            test2 = OT[2].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP;

//            test1 = OT[1].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP;
//            test2 = OT[2].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP;

            test1.Normalize();
            test2.Normalize();

            float cos = Mathf.Cos(Vector3.Angle(test1, test2) * Mathf.Deg2Rad);

            //  Use the Leg Rule (proportion).  https://mathbitsnotebook.com/Geometry/RightTriangles/RTmeanRight.html
            float hypotenuse = Vector3.Distance(OT[0].GetComponent<TreeAnchor>().CP, OT[1].GetComponent<TreeAnchor>().CP);

            cos = cos * Vector3.Distance(OT[0].GetComponent<TreeAnchor>().CP, OT[2].GetComponent<TreeAnchor>().CP);

            Vector3 direction = (OT[1].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP);
            Vector3 altPoint = OT[0].GetComponent<TreeAnchor>().CP + direction.normalized * Mathf.Abs(cos);

            ProjectionPoint = altPoint;
            PrimaryAngle = Vector3.Angle(test2, test1);
            SecondaryAngle = Vector3.Angle(OT[2].GetComponent<TreeAnchor>().CP - OT[1].GetComponent<TreeAnchor>().CP, OT[0].GetComponent<TreeAnchor>().CP - OT[1].GetComponent<TreeAnchor>().CP);
            return altPoint;
        }

        public void UpdateDatums()
        {
            List<GameObject> OT = new List<GameObject>();
            OT = transform.GetComponent<Treehouse>().OrderedTrees;

            DatumDistance = Vector3.Distance(OT[0].GetComponent<TreeAnchor>().CP, OT[1].GetComponent<TreeAnchor>().CP) - OT[0].GetComponent<TreeAnchor>().PlateOffset - OT[0].GetComponent<TreeAnchor>().PlateOffset;

            GetAlttitudePoint();
        }
    }
}

