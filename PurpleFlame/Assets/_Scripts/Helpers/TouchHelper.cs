using UnityEngine;
using System.Collections;
using Lean.Touch;

namespace BasHelpers
{
    public static class TouchHelper
    {
        // test if a finger touches an object using raycasts
        public static bool TouchesObject(Lean.Touch.LeanFinger finger, GameObject gameObject, float maxDistance = float.PositiveInfinity)
        {
            LayerMask layermask = UnityEngine.Physics.DefaultRaycastLayers;
            return TouchesObjectInLayers(finger, gameObject, layermask, maxDistance);
        }

        // test if a finger touches an object using raycasts fr a given camera
        public static bool TouchesObjectLean(LeanFinger finger, GameObject gameObject, Camera camera, float maxDistance = float.PositiveInfinity)
        {
            LayerMask layermask = UnityEngine.Physics.DefaultRaycastLayers;
            return TouchesObjectInLayers(finger, gameObject, layermask, maxDistance, camera);
        }

        // test if a finger touches an object using raycasts
        public static bool TouchesObjectInLayers(LeanFinger finger, GameObject gameObject, LayerMask layerMask, float maxDistance = float.PositiveInfinity, Camera camera = null)
        {
            if (gameObject != null)
            {
                // Raycast information
                var ray = finger.GetRay(camera);
                var hit = default(RaycastHit);

                // Was this finger pressed down on a collider?
                if (Physics.Raycast(ray, out hit, maxDistance, layerMask) == true)
                {
                    // Was that collider this one?
                    return hit.collider.gameObject == gameObject;
                }
            }
            return false;
        }

        // return the object that was touched (if any) using raycasts
        public static GameObject GetTouchedObject(LeanFinger finger)
        {
            LayerMask layermask = UnityEngine.Physics.DefaultRaycastLayers;
            return GetTouchedObjectInLayers(finger, layermask);
        }

        // return the object that was touched (if any) using raycasts
        public static GameObject GetTouchedObjectInLayers(LeanFinger finger, LayerMask layerMask)
        {
            var ray = finger.GetRay();
            var hit = default(RaycastHit);

            // Was this finger pressed down on a collider?
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask) == true)
            {
                return hit.collider.gameObject;
            }
            return null;
        }
    }
}