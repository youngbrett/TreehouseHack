using System;
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

        public float PlateWidth = .1f;              // Width of the deck plates.
        private Bounds PlateBounds;                      // This is the extents of the plate.
        private float DatumDistance = 0;            // Longest span.
        private int ActivePlates = 0;                // Plates currently on the deck.
        private int ActiveBeams = 0;                // Plates currently on the deck.
        private int ActiveRails = 0;
        private Vector3 ProjectedIntersection = new Vector3();

        private Vector3 ProjectionPoint;             // The orthogonal projection point, in world space.
        private Vector3 test1;                       // DEBUGGING
        private Vector3 test2;                       // DEBUGGING
        private Vector3 test3;                       // DEBUGGING
        private GameObject testsphere;               // DEBUGGING
        private Vector3 GenerationDirection;         //  Change to private after DEBUG>
        private Vector3 Orientation;                 //  Change to private after DEBUG>

        private Vector3 OriginPoint;
        private float DeckLength;
        private Vector3 DeckForwardDirection;

        public  float PrimaryAngle;
        public float SecondaryAngle;

        public float MaxOverhang = 0.6096f;         // 2 feet = 0.6096m
        public float MaxDeckWidth = (2.4384f / 2f);
        public float MaxDeckLength = 2.4384f;
        public float MaxBeamLength = 2.4384f * 14f / 8f;
        public float PlateLength = 2.4384f;

        public Bounds BeamBounds;

        private void Awake()
        {
            th = gameObject.GetComponent<Treehouse>();

            if (PlateType == null) PlateType = Resources.Load("Prefabs/Plate") as GameObject;          //  If no plate loaded, do a 2x4.
            if (BeamType == null) BeamType = Resources.Load("Prefabs/Beam") as GameObject;
            if (RailType == null) RailType = Resources.Load("Prefabs/Rail") as GameObject;

            PlateWidth = PlateType.GetComponent<Renderer>().bounds.size.x;                          //  Set the size, bounds of the Plate.
            PlateBounds = PlateType.GetComponent<Renderer>().bounds;
            BeamBounds = CalculateBounds(BeamType); 
            /*
            Debug.Log("X: " + BeamBounds.size.x);
            Debug.Log("Y: " + BeamBounds.size.y);
            Debug.Log("Z: " + BeamBounds.size.z);
            */
          //  testsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
           // testsphere.name = "Projected Origin";                       /// DEBUGGING
        }

        public void DrawDeck()
        {
            Vector3 oldPosition = this.transform.position;
            Quaternion oldRotation = this.transform.rotation;

            this.transform.position = Vector3.zero;
            this.transform.rotation = Quaternion.identity;

            if (th.Trees.Count < 3) { Debug.Log("Not enough trees."); return; }

            if (!ManageBeams())
                {
                Debug.Log("Beams are overspan.");
                return;
                }

            RectDeck(PlateBounds.size.z / 2, DeckLength);

            this.transform.position = OriginPoint;
            this.transform.rotation = Quaternion.LookRotation(DeckForwardDirection, Vector3.up);
        }

        public void RectDeck(float width, float length)
        {
            ActivePlates = Mathf.FloorToInt(length / PlateWidth);
            ManagePlates();
            ManageRails();

            void ManagePlates()
            {
                if (this.Plates.Count == 0)
                {
                    for (int i = 0; i < ActivePlates; i++)
                    {
                        AddPlate();
                    }
                }
                else
                {
                    int delta = ActivePlates - this.Plates.Count;
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
                }

                for (int i = 0; i < ActivePlates; i++)
                {
                    Plates[i].transform.position = new Vector3(width / 2, PlateBounds.size.y * (-0.5f), PlateBounds.size.x * (i + 0.5f));
                    Plates[i].transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
                    Plates[i].transform.localScale = new Vector3(1, 1, width / PlateBounds.size.z);
                }

                void AddPlate()
                {
                    GameObject NewPlate = Instantiate(PlateType);
                    NewPlate.transform.parent = this.transform;
                    Plates.Add(NewPlate.GetComponent<Plate>());
                }
            }

            void ManageRails()
            {
                if (this.Rails.Count < 4)
                {
                    int total = this.Rails.Count;
                    for (int i = 0; i < (4 - total); i++)
                    {
                        AddRail();
                    }
                    ActiveRails = this.Rails.Count;
                }

                PlaceRail(Rails[0], new Vector3(-0.5f * (width + PlateWidth), th.RailElevation + PlateBounds.size.y / 2, -PlateWidth), new Vector3(-0.5f * (width + PlateWidth), th.RailElevation + 0.5f * PlateBounds.size.y, +PlateWidth + ActivePlates * PlateWidth));
                PlaceRail(Rails[1], new Vector3(0.5f * (width + PlateWidth), th.RailElevation + PlateBounds.size.y / 2, -PlateWidth), new Vector3(0.5f * (width + PlateWidth), th.RailElevation + 0.5f * PlateBounds.size.y, +PlateWidth + ActivePlates * PlateWidth));
                PlaceRail(Rails[2], new Vector3(-0.5f * (width), th.RailElevation + 0.5f * PlateBounds.size.y, -0.5f * PlateWidth), new Vector3(0.5f * (width), th.RailElevation + 0.5f * PlateBounds.size.y, -0.5f * PlateWidth));
                PlaceRail(Rails[3], new Vector3(-0.5f * (width), th.RailElevation + 0.5f * PlateBounds.size.y, +0.5f * PlateWidth + ActivePlates * PlateWidth), new Vector3(0.5f * (width), th.RailElevation + 0.5f * PlateBounds.size.y, +0.5f * PlateWidth + ActivePlates * PlateWidth));

                void AddRail()
                {
                    GameObject NewRail = Instantiate(RailType);
                    NewRail.transform.parent = this.transform;
                    Rails.Add(NewRail.GetComponent<Rail>());
                }

                void PlaceRail(Rail rail, Vector3 start, Vector3 end)
                {
                    rail.transform.localScale = Vector3.one;
                    rail.transform.position = start;
                    rail.transform.rotation = Quaternion.LookRotation(end - start, Vector3.up);
                    rail.transform.localScale = new Vector3(1, 1, (Vector3.Distance(end, start)) / PlateBounds.size.z);
                    rail.RailLength = Vector3.Distance(end, start);
                }
            }
        }

        public bool ManageBeams()
        {
            bool result = true;

            List<GameObject> OT = new List<GameObject>();
            OT = transform.GetComponent<Treehouse>().OrderedTrees;

            if (this.Beams.Count < 3)
            {
                int total = this.Beams.Count;

                for (int i = 0; i < (3 - total); i++)
                {
                    AddBeam();
                }
                ActiveBeams = this.Beams.Count;
            }
            MaxDeckWidth = (0.5f * 2.4384f);
            float BeamDepth = BeamBounds.size.y;
            Vector3 BeamDepthVector = new Vector3(0f, -BeamDepth, 0f);

            PlaceBeam(Beams[0], th.OrderedTrees[1].GetComponent<TreeAnchor>().CP + BeamDepthVector, th.OrderedTrees[2].GetComponent<TreeAnchor>().CP + BeamDepthVector);

            if (Vector3.Distance(th.OrderedTrees[1].GetComponent<TreeAnchor>().CP, th.OrderedTrees[2].GetComponent<TreeAnchor>().CP) > MaxBeamLength)
            {
                result = false;
                return result;
            }

            PrimaryAngle = 0.5f * Vector3.Angle(th.OrderedTrees[1].GetComponent<TreeAnchor>().CP - th.OrderedTrees[0].GetComponent<TreeAnchor>().CP, th.OrderedTrees[2].GetComponent<TreeAnchor>().CP - th.OrderedTrees[0].GetComponent<TreeAnchor>().CP);

            Vector3 BeamCP1 = new Vector3();
            Vector3 BeamCP2 = new Vector3();



            Vector3 MidPoint1to2 = 0.5f * (th.OrderedTrees[1].GetComponent<TreeAnchor>().CP + th.OrderedTrees[2].GetComponent<TreeAnchor>().CP);

            float BeamAngle = Vector3.Angle(th.OrderedTrees[0].GetComponent<TreeAnchor>().CP - MidPoint1to2, th.OrderedTrees[1].GetComponent<TreeAnchor>().CP - MidPoint1to2);
            BeamAngle = BeamAngle - 90f;

            if ((Vector3.Distance(MidPoint1to2, th.OrderedTrees[1].GetComponent<TreeAnchor>().CP)) * (Mathf.Cos((BeamAngle) * Mathf.Deg2Rad)) > MaxDeckWidth * 0.5f)
            {

                float BeamConnectionHypotenuse = (0.5f * MaxDeckWidth) / (Mathf.Cos((BeamAngle) * Mathf.Deg2Rad));
                Vector3 BeamVector = th.OrderedTrees[1].GetComponent<TreeAnchor>().CP - MidPoint1to2;
                BeamVector.Normalize();
                BeamCP1 = MidPoint1to2 - BeamConnectionHypotenuse * BeamVector;
                BeamCP2 = MidPoint1to2 + BeamConnectionHypotenuse * BeamVector;
            }
            else
            {
                BeamCP1 = th.OrderedTrees[1].GetComponent<TreeAnchor>().CP;
                BeamCP2 = th.OrderedTrees[2].GetComponent<TreeAnchor>().CP;
            }

            Vector3 point1 = th.OrderedTrees[0].GetComponent<TreeAnchor>().Anchor.transform.position + th.OrderedTrees[0].GetComponent<TreeAnchor>().CPTreeOffset * th.OrderedTrees[0].GetComponent<TreeAnchor>().Anchor.transform.right;
            Vector3 point2 = th.OrderedTrees[0].GetComponent<TreeAnchor>().Anchor.transform.position + th.OrderedTrees[0].GetComponent<TreeAnchor>().CPTreeOffset * -th.OrderedTrees[0].GetComponent<TreeAnchor>().Anchor.transform.right;

            Vector3 closestOT1 = FindClosestPoint(point1, point2, BeamCP1);
            Vector3 closestOT2 = FindClosestPoint(point1, point2, BeamCP2);

            PlaceBeam(Beams[1], closestOT1, BeamCP1);
            PlaceBeam(Beams[2], closestOT2, BeamCP2);

            LineIntersection(BeamCP1, closestOT1 - BeamCP1, BeamCP2, closestOT2 - BeamCP2, ref ProjectedIntersection);

            PrimaryAngle = 0.5f * Vector3.Angle(BeamCP1 - ProjectedIntersection, BeamCP2 - ProjectedIntersection);

            Vector3 AvgVector = (BeamCP1 + BeamCP2) / 2f - ProjectedIntersection; // + (OT[2].GetComponent<TreeAnchor>().CP - OT[0].GetComponent<TreeAnchor>().CP)) / 2;
            AvgVector.Normalize();

            OriginPoint = MidPoint1to2;

            Vector3 Fwd = th.OrderedTrees[0].GetComponent<TreeAnchor>().Anchor.transform.position - MidPoint1to2;
            Fwd.Normalize();

            float len = Vector3.Distance(ProjectedIntersection + PlateBounds.size.z / 2 / Mathf.Tan(PrimaryAngle * Mathf.Deg2Rad) * AvgVector, OriginPoint);
            if (len > PlateLength) len = PlateLength;

            DeckLength = len;
            DeckForwardDirection = Fwd;
            return result;

            ///  FUNCTIONS....
            bool LineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, ref Vector3 intersection)
            {
                float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
                float x1lo, x1hi, y1lo, y1hi;

                Ax = p2.x - p1.x;
                Bx = p3.x - p4.x;

                // X bound box test/
                if (Ax < 0)
                {
                    x1lo = p2.x; x1hi = p1.x;
                }
                else
                {
                    x1hi = p2.x; x1lo = p1.x;
                }

                if (Bx > 0)
                {
                    if (x1hi < p4.x || p3.x < x1lo) return false;
                }
                else
                {
                    if (x1hi < p3.x || p4.x < x1lo) return false;
                }

                Ay = p2.z - p1.z;
                By = p3.z - p4.z;

                // Y bound box test//
                if (Ay < 0)
                {
                    y1lo = p2.z; y1hi = p1.z;
                }
                else
                {
                    y1hi = p2.z; y1lo = p1.z;
                }

                if (By > 0)
                {
                    if (y1hi < p4.z || p3.z < y1lo) return false;
                }
                else
                {
                    if (y1hi < p3.z || p4.z < y1lo) return false;
                }

                Cx = p1.x - p3.x;
                Cy = p1.z - p3.z;
                d = By * Cx - Bx * Cy;  // alpha numerator//
                f = Ay * Bx - Ax * By;  // both denominator//

                // alpha tests//
                if (f > 0)
                {
                    if (d < 0 || d > f) return false;
                }
                else
                {
                    if (d > 0 || d < f) return false;
                }

                e = Ax * Cy - Ay * Cx;  // beta numerator//

                // beta tests //

                if (f > 0)
                {
                    if (e < 0 || e > f) return false;
                }
                else
                {
                    if (e > 0 || e < f) return false;
                }

                // check if they are parallel
                if (f == 0) return false;
                // compute intersection coordinates //
                num = d * Ax; // numerator //
                              //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //
                              //    intersection.x = p1.x + (num+offset) / f;
                intersection.x = p1.x + num / f;

                num = d * Ay;
                //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
                //    intersection.y = p1.y + (num+offset) / f;
                intersection.z = p1.z + num / f;
                intersection.y = p1.y;

                return true;
            }

            Vector3 FindClosestPoint(Vector3 Point1, Vector3 Point2, Vector3 Start)
            {
                float d1 = Vector3.Distance(Point1, Start);
                float d2 = Vector3.Distance(Point2, Start);

                if (d1 < d2) return Point1;
                else return Point2;
            }

            void AddBeam()
            {
                GameObject NewBeam = Instantiate(BeamType);
                //NewBeam.transform.parent = this.transform;
                Beams.Add(NewBeam.GetComponent<Beam>());
            }

            void PlaceBeam(Beam beam, Vector3 start, Vector3 end)
            {
                beam.transform.position = start;
                beam.transform.rotation = Quaternion.LookRotation(end - start);
                beam.transform.localScale = new Vector3(1, 1, Vector3.Distance(end, start) / 2.4384f);
            }

        } 

        public Bounds CalculateBounds(GameObject UnboundedParent)
        {
            Bounds bounds = new Bounds();
            bounds.size = Vector3.zero; // reset
            Renderer[] renderers = UnboundedParent.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                bounds.Encapsulate(rend.bounds);
            }
            return bounds;
        }
    }
}

