using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeHouseHack
{
    public class Deck : MonoBehaviour
    {
        public List<Plate> Plates = new List<Plate>();   //  Make private after testing.

        public int BeamPlateOverlap = 1;
        public GameObject PlateType;
        public float PlateWidth = .1f;
        private Bounds Offset;
        private float DatumDistance = 0;
        private Treehouse th = new Treehouse();
        public int ActivePlates = 0;
        public Vector3 ProjectionPoint;

        public Vector3 GenerationDirection;
        public Vector3 Orientation;

        private void Awake()
        {
            th = gameObject.GetComponent<Treehouse>();

            if (PlateType == null)  PlateType = Resources.Load("Prefabs/Plate") as GameObject;
            PlateWidth = PlateType.GetComponent<MeshRenderer>().bounds.size.x;
            Offset = PlateType.GetComponent<MeshRenderer>().bounds;
        }

        public  void ManagePlates()
        {
            if (th.Trees.Count < 3)
            {
                Debug.Log("Not enough trees.");
                return;
            }

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
                //int delta = Mathf.RoundToInt(DatumDistance / PlateWidth) - ActivePlates;
                int delta = Mathf.RoundToInt(Vector3.Distance(th.Trees[1].GetComponent<TreeAnchor>().CP,th.Trees[0].GetComponent<TreeAnchor>().CP)/ PlateWidth) - ActivePlates;

                for (int j = 0; j < delta; j++)           //  Add Plates.
                {
                    AddPlate();
                }
                for (int j = 0; j > delta; j--)             // Lose Plates.
                {
                    Plates[Plates.Count - 1 + j].transform.position = new Vector3(0, -10f, 0);
                }
                ActivePlates = Mathf.RoundToInt(Vector3.Distance(th.Trees[1].GetComponent<TreeAnchor>().CP, th.Trees[0].GetComponent<TreeAnchor>().CP) / PlateWidth);
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
            th = gameObject.GetComponent<Treehouse>();          //  Doing this to verify I don't have a weird ref problem.

            for (int i = 0; i < Plates.Count; i++)
            {
                int length = 16;

                GenerationDirection = th.Trees[1].GetComponent<TreeAnchor>().CP - th.Trees[0].GetComponent<TreeAnchor>().CP;
                GenerationDirection.Normalize();

                Orientation = th.Trees[2].GetComponent<TreeAnchor>().CP - ProjectionPoint;
                Orientation.Normalize();

                Debug.Log(Quaternion.LookRotation(GenerationDirection).eulerAngles);

                if (i < ActivePlates)
                {
                    Plates[i].transform.forward = Orientation;
                    Plates[i].transform.position = th.Trees[0].GetComponent<TreeAnchor>().CP + Offset.size.x * GenerationDirection * i;
                }
                else
                {
                    Plates[1].transform.position = new Vector3(0, -50, 0);
                }

                //Plates[i].transform.position = th.Trees[0].GetComponent<TreeAnchor>().CP + Quaternion.LookRotation(GenerateDirection) * new Vector3(Offset.size.x * i, 1, -Offset.size.z / 16f * BeamPlateOverlap);
                //Plates[i].transform.position = this.transform.position + this.transform.rotation * new Vector3(Offset.size.x * i + th.Trees[0].GetComponent<Tree>().Diameter, 0, -Offset.size.z / 16f * BeamPlateOverlap);
                //Plates[i].transform.localScale = new Vector3(1f, 1f, length / 16f);
            }
        }

        public Vector3 GetAlttitudePoint()
        {
            //  Use the Leg Rule (proportion).  https://mathbitsnotebook.com/Geometry/RightTriangles/RTmeanRight.html
            float hypotenuse = Vector3.Distance(th.Trees[0].GetComponent<TreeAnchor>().CP, th.Trees[1].GetComponent<TreeAnchor>().CP);
            float projection = Mathf.Pow(Vector3.Distance(th.Trees[2].GetComponent<TreeAnchor>().CP, th.Trees[0].GetComponent<TreeAnchor>().CP),2) / hypotenuse ;       // This is wrong.

            //Debug.Log("h: " + hypotenuse.ToString() + "p: " + projection.ToString());
            Vector3 direction = (th.Trees[1].GetComponent<TreeAnchor>().CP - th.Trees[0].GetComponent<TreeAnchor>().CP);
            direction.Normalize();
            Vector3 altPoint = th.Trees[0].GetComponent<TreeAnchor>().CP + direction * projection;

            return altPoint;
        }

        public void UpdateDatums()
        {
            DatumDistance = Vector3.Distance(th.Trees[1].GetComponent<TreeAnchor>().CP, th.Trees[0].GetComponent<TreeAnchor>().CP) - th.Trees[0].GetComponent<TreeAnchor>().PlateOffset - th.Trees[1].GetComponent<TreeAnchor>().PlateOffset;
            if (DatumDistance != 0)  ProjectionPoint = GetAlttitudePoint();
        }
    }
}

