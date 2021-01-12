using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using Dennis;
using TouchBehaviours;
using BasHelpers;

namespace CameraSystem
{
    public class CameraManager : LeanDrag
    {
        public float RaycastRange = 100f;

        private Ray GenerateMouseRay()
        {
            Vector3 mousePositionFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);

            Vector3 mousePositionNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

            Vector3 mousePositionF = Camera.main.ScreenToWorldPoint(mousePositionFar);
            Vector3 mousePositionN = Camera.main.ScreenToWorldPoint(mousePositionNear);

            Ray r = new Ray(mousePositionN, mousePositionF - mousePositionN);
            return r;
        }

        protected override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);

            //Generate a ray from MousePosition(touchPosition 0 )
            Ray mouseRay = GenerateMouseRay();

            //Create a raycasthit
            RaycastHit hit;

            //Shoot a Raycast from mouseRay with mouseRay direction towards the Touchablelayers given in the GameManager.
            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, RaycastRange, GameManager.Instance.TouchableLayers))
            {
                Debug.Log("RAYCAST");
                if (hit.transform.gameObject != null && hit.transform.gameObject.HasComponent<CameraTouchTarget>())
                {

                }
            }

        }
    }
}
