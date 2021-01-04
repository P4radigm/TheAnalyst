using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

namespace BasHelpers
{
    public static class CustomMath
    {
        // angle between 2 vectors
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        public static float AngleSigned(float a, float b)
        {
            float difference = b - a;
            while (difference < -180) difference += 360;
            while (difference > 180) difference -= 360;
            return difference;
        }

        public enum DirectionVector { Forward, Right, Up, Left, Down, Back }

        public static Vector3 GetTransformDirection(Transform transf, DirectionVector direction)
        {
            Vector3 dir;
            switch (direction)
            {
                case DirectionVector.Forward: dir = transf.forward; break;
                case DirectionVector.Right: dir = transf.right; break;
                case DirectionVector.Up: dir = transf.up; break;
                case DirectionVector.Down: dir = -transf.up; break;
                case DirectionVector.Left: dir = -transf.right; break;
                default: dir = -transf.forward; break; //back
            }
            return dir;
        }

        public static DirectionVector GetInverseDirectionVector(DirectionVector direction)
        {
            switch (direction)
            {
                case DirectionVector.Forward: return DirectionVector.Back;
                case DirectionVector.Right: return DirectionVector.Left;
                case DirectionVector.Up: return DirectionVector.Down;
                case DirectionVector.Down: return DirectionVector.Up;
                case DirectionVector.Left: return DirectionVector.Right;
                default /* back */ : return DirectionVector.Forward;
            }
        }

        public static Vector3 VectorModulo(Vector3 vector, float mod)
        {
            Vector3 temp = vector;
            temp.x %= 360;
            temp.y %= 360;
            temp.z %= 360;
            return temp;
        }


        public static float SquaredDistance(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        public static Vector3 QuadraticBezier(Vector3 startPos, Vector3 midPos, Vector3 endPos, float time)
        {
            float t1 = 1 - time;
            return (t1 * t1 * startPos) + (2 * t1 * time * midPos) + (time * time * endPos);
        }
    }
}
