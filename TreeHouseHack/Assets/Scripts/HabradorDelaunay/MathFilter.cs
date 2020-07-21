using System;
using System.Collections.Generic;
using System.Linq;
using TreehouseHack;
using UnityEngine;

namespace HabradorDelaunay
{
    internal static class MathFilter
    {
        internal static double CalculateDistance(Vector3 A, Vector3 B)
        {
            return Math.Sqrt(Math.Pow((B.x - A.x), 2) + Math.Pow((B.z - A.z), 2));
        }

        internal static double TriangleArea(Triangle tri)
        {
            Vector3 A = tri.v1.position;
            Vector3 B = tri.v2.position;
            Vector3 C = tri.v3.position;

            


            return Math.Abs(0.5 * ((A.x * (B.z - C.z)) + (B.x * (C.z - A.z)) + (C.x * (A.z - B.z))));
        }

        internal static List<double> AngleDegrees(Triangle tri)
        {
            Vector3 A = tri.v1.position;
            Vector3 B = tri.v2.position;
            Vector3 C = tri.v3.position;

            double a = CalculateDistance(B, C);
            double b = CalculateDistance(C, A);
            double c = CalculateDistance(A, B);

            double cos_A_angle = (((Math.Pow(b, 2)) + (Math.Pow(c, 2)) - (Math.Pow(a, 2))) / (2 * b * c));
            double cos_B_angle = (((Math.Pow(c, 2)) + (Math.Pow(a, 2)) - (Math.Pow(b, 2))) / (2 * c * a));
            double cos_C_angle = (((Math.Pow(a, 2)) + (Math.Pow(b, 2)) - (Math.Pow(c, 2))) / (2 * a * b));

            double A_angle = Math.Acos(cos_A_angle) * (180 / Math.PI);
            double B_angle = Math.Acos(cos_B_angle) * (180 / Math.PI);
            double C_angle = Math.Acos(cos_C_angle) * (180 / Math.PI);

            return new List<double> { A_angle, B_angle, C_angle };

        }

        internal static bool LengthFilter(Triangle tri, uint MaxLength)
        {
            // Length of zero means no length filter
            if (MaxLength == 0)
            {
                return true;
            }

            Vector3 P1 = tri.v1.position;
            Vector3 P2 = tri.v2.position;
            Vector3 P3 = tri.v3.position;

            double a = MathFilter.CalculateDistance(P1, P2);
            double b = MathFilter.CalculateDistance(P2, P3);
            double c = MathFilter.CalculateDistance(P3, P1);

            if (a > MaxLength || b > MaxLength || c > MaxLength)
            {
                return false;
            }
            return true;
        }

        internal static double[] AreaSpan(List<Triangle> triangulation, 
                                            uint MaxLength, 
                                            double MinAngle, 
                                            AreaRange RelativeArea)
        {
            double[] check_range;

            List<double> areas = new List<double>();


            /* Here we get the areas for the triangles that PASS the angle and
             * length filter. That way, the min area will be a valid triangle.
             * We check those areas against the range later. */
            foreach (Triangle tri in triangulation)
            {
                List<double> TriAngles = MathFilter.AngleDegrees(tri);

                if (LengthFilter(tri, MaxLength) && 
                    TriAngles.TrueForAll(angle => angle >= MinAngle))
                {
                    areas.Add(MathFilter.TriangleArea(tri));
                }
            }

            double area_range = areas.Max() - areas.Min();

            double min_area = areas.Min();
            double max_area = areas.Max();
            double low_mid = min_area + (area_range / 5);
            double high_mid = min_area + ((area_range / 5)*3);

            double[] no_range = new double[] { min_area, max_area };

            double[] low_range = new double[] { min_area, low_mid };
            double[] mid_range = new double[] { low_mid, high_mid };
            double[] high_range = new double[] { high_mid, max_area };

            switch (RelativeArea)
            {
                case AreaRange.Low:
                    check_range = low_range;
                    break;
                case AreaRange.Mid:
                    check_range = mid_range;
                    break;
                case AreaRange.High:
                    check_range = high_range;
                    break;
                case AreaRange.Surprise:
                    check_range = no_range; // Not implemented yet
                    break;
                case AreaRange.None:
                default:
                    check_range = no_range;
                    break;

            }

            return check_range;
        }

    }
}