using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasHelpers
{
    /// <summary>
    /// static extensions for methods I often use
    /// </summary>
    public static class ExtensionMethods
    {
        public static RectTransform LerpRectTransform(this RectTransform rectTransform, MonoBehaviour owner, Vector3 targetPosition, float speed)
        {
            return ActivateCoroutineLerpRectTransformPositions(rectTransform, owner, targetPosition, speed);
        }

        public static Transform LerpTransform(this Transform currentTransform, MonoBehaviour owner, Vector3 targetPosition, float speed)
        {
            return ActivateCoroutineLerpTransformPositions(currentTransform, owner, targetPosition, speed);
        }

        public static GameObject DeactivateAfterTime(this GameObject currentGameObject, MonoBehaviour owner, float time)
        {
            return ActivateCoroutineDeactiateAfterTime(currentGameObject, owner, time);
        }

        public static List<T> ToList<T>(this T[] array) where T : class
        {
            List<T> output = new List<T>();
            output.AddRange(array);
            return output;
        }

        public static GameObject ActivateCoroutineDeactiateAfterTime(GameObject objToDeactivate, MonoBehaviour owner, float time)
        {
            if (objToDeactivate == null) return null;

            if(owner != null)
            {
                owner.StartCoroutine(ExtensionHelpers.DeactivateAfterTime(objToDeactivate, time));
                return objToDeactivate;
            }
            else
            {
                Debug.Log("Our Owner is null");
                return null;
            }
        }

        public static RectTransform ActivateCoroutineLerpRectTransformPositions(RectTransform rectTransform, MonoBehaviour owner, Vector3 targetPosition, float speed)
        {
            if (rectTransform == null) return null;

            if (owner != null)
            {
                owner.StartCoroutine(ExtensionHelpers.LerpRectTransformPositions(rectTransform, targetPosition, speed));
                return rectTransform;
            }
            else
            {
                Debug.Log("Our Owner is null");
                return null;
            }
        }

        public static Transform ActivateCoroutineLerpTransformPositions(Transform currentTransform, MonoBehaviour owner, Vector3 targetPosition, float speed)
        {
            if (currentTransform == null) return null;

            if (owner != null)
            {
                owner.StartCoroutine(ExtensionHelpers.LerpTransformPositions(currentTransform, targetPosition, speed));
                return currentTransform;
            }
            else
            {
                Debug.Log("Our Owner is null");
                return null;
            }
        }

        public static bool HasComponent<T>(this GameObject obj)
        {
            return obj.GetComponent(typeof(T)) != null;
        }

        public static Vector3 GetDirectionTo(this Vector3 from, Vector3 lookAt)
        {
            return lookAt - from;
        }

        public static Vector3 ToZeroY(this Vector3 vec)
        {
            return new Vector3(vec.x, 0, vec.z);
        }

        public static Vector3 ToZeroZ(this Vector3 vec)
        {
            return new Vector3(vec.x, vec.y, 0);
        }

        public static Vector3 KeepOwnY(this Vector3 vec, Vector3 newVec)
        {
            return new Vector3(newVec.x, vec.y, newVec.z);
        }

        public static Vector3 KeepYOf(this Vector3 vec, Vector3 keepYVector)
        {
            return new Vector3(vec.x, keepYVector.y, vec.z);
        }
    }
}




