using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using Dennis;
using TouchBehaviours;
using BasHelpers;

namespace CameraSystem
{
    /// <summary>
    /// Main Manager regulating camera interaction
    /// </summary>
    public class CameraManager : LeanDrag
    {
        public float InterpolationSpeed = 5f;
        public CameraMotor CameraMotor;
        public ObjectRotation ObjectRotation;
        public float RaycastRange = 100f;

        private GameObject target, lastTarget;
        private float tabCooler, tabCount;
        private CameraTouchTarget currentlyAtTarget;

        /// <summary>
        /// Create ray at mouse position
        /// </summary>
        /// <returns></returns>
        private Ray GenerateMouseRay()
        {
            Vector3 mousePositionFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);

            Vector3 mousePositionNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

            Vector3 mousePositionF = Camera.main.ScreenToWorldPoint(mousePositionFar);
            Vector3 mousePositionN = Camera.main.ScreenToWorldPoint(mousePositionNear);

            Ray r = new Ray(mousePositionN, mousePositionF - mousePositionN);
            return r;
        }

        private void Update()
        {
            //Dubble tap cooldown
            if (tabCooler > 0)
            {
                tabCooler -= 1 * Time.deltaTime;
            }
            else
            {
                tabCount = 0;
            }
        }

        /// <summary>
        /// Compare the current layer the camera motor is currently on with the target layer 
        /// Checker if we should be able to go to this layer from ...
        /// </summary>
        /// <param name="cameraLayer">The current layer the CameraMotor is on</param>
        /// <param name="targetLayer">The target layer </param>
        /// <returns></returns>
        private bool PossibleFromLayer(CameraLayer cameraLayer, CameraLayer targetLayer)
        {
            switch (cameraLayer)
            {
                case CameraLayer.VantagePoint:
                    return targetLayer.Equals(CameraLayer.Layer1) || targetLayer.Equals(CameraLayer.VantagePoint) ? true : false;
                case CameraLayer.Layer1:
                    return targetLayer.Equals(CameraLayer.Layer2) || targetLayer.Equals(CameraLayer.Layer1) ? true : false;
                case CameraLayer.Layer2:
                    return targetLayer.Equals(CameraLayer.Layer3) || targetLayer.Equals(CameraLayer.Layer2) ? true : false;
                case CameraLayer.Layer3:
                    return targetLayer.Equals(CameraLayer.Layer4) || targetLayer.Equals(CameraLayer.Layer3) ? true : false;
                default:
                    return true;
            }
        }

        
        /// <summary>
        /// Gets called every time the finger touches the screen.
        /// </summary>
        /// <param name="finger"></param>
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
                    target = hit.transform.gameObject;
                    CameraTouchTarget targetInspect = target.GetComponent<CameraTouchTarget>();
                    CameraLayer targetCameraLayer = targetInspect.GetCameraLayer();
                    
                    if(!PossibleFromLayer(CameraMotor.CurrentCameraLayer, targetCameraLayer))
                    {
                        return;
                    }

                    if (target != lastTarget)
                    {
                        lastTarget = target;
                    }

                    if (tabCooler > 0 && tabCount > 1)
                    {
                        //has dubbletapped

                        DubbleTapedOnInspectable();

                    }
                    else
                    {
                        //We have tabbed once

                        tabCooler = 0.5f;
                        tabCount += 1;
                        if (tabCount > 1)
                            DubbleTapedOnInspectable();
                    }
                }
            }

        }
        bool wachten = false;
        public void ReturnToLastCameFrom()
        {
            if (wachten) return;
            switch(CameraMotor.GetCurrentCameraLayer())
            {
                case CameraLayer.Layer1:
                    CameraMotor.SetCurrentCameraLayer(CameraLayer.VantagePoint);
                    break;
                case CameraLayer.Layer2:
                    CameraMotor.SetCurrentCameraLayer(CameraLayer.Layer1);
                    break;
                case CameraLayer.Layer3:
                    CameraMotor.SetCurrentCameraLayer(CameraLayer.Layer2);
                    break;
                case CameraLayer.Layer4:
                    CameraMotor.SetCurrentCameraLayer(CameraLayer.Layer3);
                    break;

            }
            wachten = true;
            //if(PossibleFromLayer(target.GetComponent<CameraTouchTarget>().GetCameraLayer(), CameraMotor.GetCurrentCameraLayer()))
            //
            CameraMotor.MoveToLastTarget(5);
            StartCoroutine(Wachtff()); 
            //}
        }

        private IEnumerator Wachtff()
        {
            wachten = true;
            yield return new WaitForSeconds(2f);
            wachten = false;
        }

        /// <summary>
        /// When we dubble taped on an inspectable we need to do some stuff
        /// </summary>
        private void DubbleTapedOnInspectable()
        {
            if (target == null) return;

            //Dissable ObjectRotation because we are interpolating towards inspectable
            ObjectRotation.enabled = false;

            //We want to be looking at the target we are moving towards
            if (CameraMotor.enabled == false)
                CameraMotor.enabled = true;

            lastTarget = null;

            CameraTouchTarget targetInspect = target.GetComponent<CameraTouchTarget>();

            //Get the target his preferred cam position we want to be moving to
            Transform targetPrefferedCamPosition = targetInspect.PrefCamTransformPosRot;

            //Move towards new target preferred cam position
            if (targetPrefferedCamPosition != null)
                CameraMotor.MoveToNewTarget(targetInspect, InterpolationSpeed);

            //CameraFollowTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            //Lock camera when the inspectable has Locked state
            switch (targetInspect.GetLockState())
            {
                case LockState.Locked:
                    {
                        tabCount = 0;
                        return;
                    }
                case LockState.NoLock:
                    StartCoroutine(DissableLookat());
                    break;

            }
        }

        /// <summary>
        /// Dissables the lookat and enables the ObjectRotation of camera using swipe
        /// </summary>
        /// <returns></returns>
        private IEnumerator DissableLookat()
        {
            yield return new WaitForSeconds(1f);
            //ObjectRotation.enabled = true;
            //CameraLookAtTarget.enabled = false;
            //CameraFollowTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}
