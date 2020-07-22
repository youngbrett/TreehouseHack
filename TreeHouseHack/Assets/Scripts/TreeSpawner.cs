using HabradorDelaunay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TreeHouseHack;

namespace TreehouseHack
{
    // TODO: On neighbor triangle sides, remove rails and pickets

    public class TreeSpawner : MonoBehaviour
    {
        [Header("Spatial")]
        public float MaxCoordinate;
        public int TreeCount;

        [Header("Constraints")]
        public uint MinAngle;
        public uint MaxLength = 8;
        public AreaRange RelativeArea;

        [Header("Objects")]
        public GameObject TreePrefab;
        public GameObject DeckObject;

        private Quaternion rot = new Quaternion(); // Zero rotation

        void Start()
        {
            // Tree map, this is random, but could be a real map from a point cloud (LG)
            List<Vector3> TreeMap = GenRandomLayout();

            // Create 3D trees
            foreach (Vector3 point in TreeMap)
            {
                GameObject newTree = Instantiate(TreePrefab, point, rot);
            }

            // Make magic happen
            GenDelaunayTreeDecks(TreeMap);
        }


        private List<Vector3> GenRandomLayout()
        {
            System.Random rnd = new System.Random();

            // Random points list
            List<Vector3> points = new List<Vector3>();

            for (int n = 1; n <= TreeCount; n++)
            {
                Vector3 RandomPoint = new Vector3(Convert.ToSingle(rnd.NextDouble() * MaxCoordinate),
                                                  0,
                                                  Convert.ToSingle(rnd.NextDouble() * MaxCoordinate));
                points.Add(RandomPoint);
            }

            return points;
        }


        private void GenDelaunayTreeDecks(List<Vector3> points)
        {
            GameObject DeckNode = new GameObject("Deck Node");

            // Magic and beautiful Delaunay triangulation
            List<Triangle> triangulation = Delaunay.TriangulateByFlippingEdges(points);

            // Area filter setup
            double[] check_range = MathFilter.AreaSpan(triangulation, MaxLength, MinAngle, RelativeArea);

            foreach (Triangle tri in triangulation)
            {
                // Length constraint. if 0, no constraint
                if (MathFilter.LengthFilter(tri, MaxLength))
                {
                    List<double> TriAngles = MathFilter.AngleDegrees(tri);

                    // Angle constraint
                    if (TriAngles.TrueForAll(angle => angle >= MinAngle))
                    {
                        double area = MathFilter.TriangleArea(tri);

                        // Area constraint
                        if (check_range[0] <= area && area <= check_range[1])
                        {

                            Vector3 P1 = tri.v1.position;
                            Vector3 P2 = tri.v2.position;
                            Vector3 P3 = tri.v3.position;

                            // Brett deck
                            GameObject TreeDeck = Instantiate(DeckObject);
                            Treehouse TreehouseComp = TreeDeck.AddComponent<Treehouse>();
                            TreeDeck.AddComponent<Deck>();

                            // Deck component needs 3 GameObjects. In the future this
                            // may become 3 Vector3s.
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
}