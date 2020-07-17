using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeHouseHack
{
    public class Rail : MonoBehaviour
    {
        public List<RailPicket> Pickets = new List<RailPicket>();   //  Make private after testing.

        public GameObject PicketType;
        public int ActivePickets = 0;
        private float PicketWidth;
        private Bounds  PicketBounds;
        private Bounds RailBounds;
        public float Spacing = 0.5f;

        void Start()
        {
            if (PicketType == null) PicketType = Resources.Load("Prefabs/Picket") as GameObject;
            PicketWidth = PicketType.GetComponent<MeshRenderer>().bounds.size.x;                          //  Set the size, bounds of the Plate.
            PicketBounds = PicketType.GetComponent<MeshRenderer>().bounds;
            RailBounds = this.gameObject.GetComponent<MeshRenderer>().bounds;

            ActivePickets = 0;
            ManagePickets();
        }

        public void ManagePickets()
        {
            RailBounds = this.gameObject.GetComponent<MeshRenderer>().bounds;

            ActivePickets = Mathf.CeilToInt(RailBounds.size.z / Spacing);

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
                RailBounds = this.gameObject.GetComponent<MeshRenderer>().bounds;
                ManagePickets();
            }
        }

        void UpdateOrientation()
        {
            for (int i = 0; i < ActivePickets; i++)
            {
                float space = RailBounds.size.z / (ActivePickets - 1);
                Pickets[i].transform.rotation = this.transform.rotation * Quaternion.Euler(-Vector3.left * 90 + Vector3.forward * 90);
                Pickets[i].transform.position = this.transform.position + this.transform.forward * space * i;
                Pickets[i].transform.localScale = new Vector3(1, 1, this.transform.GetComponentInParent<Treehouse>().RailElevation * 1.1f / 2.4384f);
            }
        }
    }
}