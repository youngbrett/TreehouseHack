using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeHouseHack
{
    public class Rail : MonoBehaviour
    {
        public List<RailPicket> Pickets = new List<RailPicket>();   //  Make private after testing.
        public GameObject PicketType;
        public float Spacing = 0.5f;
        public float RailLength;

        private int ActivePickets = 0;
        private float PicketWidth;
        private Bounds  PicketBounds;
        private Bounds RailBounds;

        void Start()
        {
            if (PicketType == null) PicketType = Resources.Load("Prefabs/Picket") as GameObject;
            PicketWidth = PicketType.GetComponent<Renderer>().bounds.size.x;                          //  Set the size, bounds of the Plate.
            PicketBounds = PicketType.GetComponent<Renderer>().bounds;
            RailBounds = this.gameObject.GetComponent<Renderer>().bounds;

            ActivePickets = 0;
            ManagePickets();
        }

        public void ManagePickets()
        {
            //RailBounds = this.gameObject.GetComponent<Renderer>().bounds;

            //ActivePickets = Mathf.CeilToInt(RailBounds.size.z / Spacing) + 1;
            ActivePickets = Mathf.CeilToInt(RailLength / Spacing) + 1;

            int delta = ActivePickets - Pickets.Count;

            if (this.Pickets.Count == 0)
            {
                for (int i = 0; i < ActivePickets; i++)
                {
                    AddPicket();
                }
            }
            else
            {
                if (delta < 0)
                {
                    for (int j = 0; j > delta; j--)             // Lose Plates.
                    {
                        Pickets[Pickets.Count - 1 + j].transform.position = new Vector3(0, -10f, 0);
                    }
                }
                else if (delta > 0)
                {
                    for (int j = 0; j < delta; j++)           //  Add Plates.
                    {
                        AddPicket();
                        Debug.Log("Picket Added");
                    }
                }

            }

            UpdateOrientation();

            void AddPicket()
            {
                GameObject NewPicket = Instantiate(PicketType);
                Pickets.Add(NewPicket.GetComponent<RailPicket>());
            }
        }

        void LateUpdate()
        {
            if (this.transform.hasChanged) {
                //RailBounds = this.gameObject.GetComponent<Renderer>().bounds;
                ManagePickets();
            }
        }

        void UpdateOrientation()
        {
            for (int i = 0; i < ActivePickets; i++)
            {
                //float space = RailBounds.size.z / (ActivePickets - 1);
                float space = RailLength / (ActivePickets - 1);

                Pickets[i].transform.rotation = this.transform.rotation * Quaternion.Euler(-Vector3.left * 90 + Vector3.forward * 90);

                if (i == 0) { Pickets[i].transform.position = this.transform.position; }
                else { Pickets[i].transform.position = this.transform.position + i * space * this.transform.forward; }

               Pickets[i].transform.localScale = new Vector3(1, 1, this.transform.GetComponentInParent<Treehouse>().RailElevation * 1.1f / 2.4384f);
            }
        }
    }
}