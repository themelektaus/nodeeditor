﻿using UnityEngine;

namespace NodeEditor.DataRenderer2D
{
    public static class Curve
    {
        public static Vector3 Auto(Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t)
        {
            t = Mathf.Clamp01(t);
            if (c0 == p0 && c1 == p1)
                return Vector3.Lerp(p0, p1, t);

            if (c0 == p0 || c1 == p1)
                return Quadratic(p0, c0 == p0 ? c1 : c0, p1, t);

            float mt = 1f - t;
            return p0 * mt * mt * mt + 3f * c0 * mt * mt * t + 3f * c1 * mt * t * t + p1 * t * t * t;
        }
        
        public static Vector3 Auto(Point n0, Point n1, float t)
        {
            var p0 = n0.position;
            var c0 = n0.NextControlPosition;
            var c1 = n1.PreviousControlPoisition;
            var p1 = n1.position;

            return Auto(p0, c0, c1, p1, t);
        }

        public static Vector3 AutoDirection(Point n0, Point n1, float t)
        {
            var p0 = n0.position;
            var c0 = n0.NextControlPosition;
            var c1 = n1.PreviousControlPoisition;
            var p1 = n1.position;

            return AutoDirection(p0, c0, c1, p1, t);
        }

        public static Vector3 AutoDirection(Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t)
        {
            var dif0 = p0 - c0;
            var dif1 = p1 - c1;

            if (dif0 == Vector3.zero && dif1 == Vector3.zero)
                return (p1 - p0).normalized;

            if (dif0 == Vector3.zero || dif1 == Vector3.zero)
                return QuadraticDirection(p0, dif0 == Vector3.zero ? c1 : c0, p1, t);

            t = Mathf.Clamp01(t);

            float mt = 1f - t;
            return (3f * mt * mt * (c0 - p0) + 6f * mt * t * (c1 - c0) + 3f * t * t * (p1 - c1));
        }

        public static Vector3 QuadraticDirection(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            float mt = 1f - t;
            return 2f * mt * (p1 - p0) + 2f * t * (p2 - p1);
        }

        public static Vector3 Quadratic(Vector3 p0, Vector3 c, Vector3 p1, float t)
        {
            float mt = 1f - t;
            return p0 * mt * mt + 2f * c * mt * t + p1 * t * t;
        }
    }
}