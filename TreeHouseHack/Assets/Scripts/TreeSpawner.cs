using HabradorDelaunay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TreeHouseHack;

namespace TreehouseHack
{
    public class TreeSpawner : MonoBehaviour
    {
        public float MaxCoordinate;
        public int TreeCount;

        public uint MinAngle;
        public AreaRange RelativeArea;

        public GameObject TreePrefab;
        public GameObject DeckObject;

        void Start()
        {
            GenDelaunayTreeDecks();
        }


        private void GenDelaunayTreeDecks()
        {
            // Set up randomizer, zero rotation and empty deck node gameobject
            System.Random rnd = new System.Random();
            Quaternion rot = new Quaternion();
            GameObject DeckNode = new GameObject("Deck Node");

            // Random points list
            List<Vector3> points = new List<Vector3>();

            for (int n = 1; n <= TreeCount; n++)
            {
                Vector3 RandomPoint = new Vector3(Convert.ToSingle(rnd.NextDouble() * MaxCoordinate),
                                                  0,
                                                  Convert.ToSingle(rnd.NextDouble() * MaxCoordinate));

                GameObject newTree = Instantiate(TreePrefab, RandomPoint, rot);

                points.Add(RandomPoint);
            }

            // Magic and beautiful Delaunay triangulation
            List<Triangle> triangulation = Delaunay.TriangulateByFlippingEdges(points);

            // Area filter
            double[] check_range = MathFilter.AreaFilter(triangulation, MinAngle, RelativeArea);

            foreach (Triangle tri in triangulation)
            {
                double area = MathFilter.TriangleArea(tri);

                // Area check
                if (check_range[0] <= area && area <= check_range[1])
                {
                    List<double> TriAngles = MathFilter.AngleDegrees(tri);

                    // Angle check
                    if (TriAngles.TrueForAll(angle => angle >= MinAngle))
                    {
                        Vector3 P1 = tri.v1.position;
                        Vector3 P2 = tri.v2.position;
                        Vector3 P3 = tri.v3.position;

                        GameObject TreeDeck = Instantiate(DeckObject);
                        Treehouse TreehouseComp = TreeDeck.AddComponent<Treehouse>();
                        TreeDeck.AddComponent<Deck>();

                        GameObject newTreeNode1 = Instantiate(DeckNode, P1, rot);
                        GameObject newTreeNode2 = Instantiate(DeckNode, P2, rot);
                        GameObject newTreeNode3 = Instantiate(DeckNode, P3, rot);

                        List<GameObject> TreesTri = new List<GameObject>();
                        TreesTri.Add(newTreeNode1);
                        TreesTri.Add(newTreeNode2);
                        TreesTri.Add(newTreeNode3);

                        TreehouseComp.Trees = TreesTri;
                    }
                }
            }
        }
    }
}