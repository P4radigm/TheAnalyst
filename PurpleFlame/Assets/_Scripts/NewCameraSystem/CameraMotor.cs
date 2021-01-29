using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasHelpers;
using TouchBehaviours;

namespace CameraSystem
{
    public enum CameraLayer
    {
        VantagePoint,
        Layer1,
        Layer2,
        Layer3,
        Layer4
    }

    /// <summary>
    /// The motor used by the Camera himself
    /// </summary>
    public class CameraMotor : MonoBehaviour
    {
        /// <summary>
        /// The layer the Camera is currently at
        /// </summary>
        public CameraLayer CurrentCameraLayer = CameraLayer.VantagePoint;

        /// <summary>
        /// Last known came from point
        /// </summary>
        public CameraTouchTarget CameFromPoint;


        public CameraTouchTarget GotoPoint;
        public CameraTouchTarget StartPoint;
        public Transform InitialStart;

        private Transform lastTarget;
        private CameraTouchTarget tmpTarget;
        private Dictionary<Transform, bool> visitedTargets = new Dictionary<Transform, bool>();

        public void SetCurrentCameraLayer(CameraLayer incoming)
        {
            CurrentCameraLayer = incoming;
        }

        public CameraLayer GetCurrentCameraLayer()
        {
            return CurrentCameraLayer;
        }

        private void Start()
        {
            transform.position = InitialStart.position;
            //Set initalStart to be always false because we are always able to return to this spot
            visitedTargets.Add(InitialStart, false);
        }

        /// <summary>
        /// Move the camera towards given target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="time"></param>
        public void MoveToNewTarget(CameraTouchTarget target, float time)
        {
            /*
            foreach(Transform cameFrom in cameFroms)
            {
                if(target.GetComponent<CameraTouchTarget>().CameFrom==cameFrom)
                {
                    able = true;
                    lastTarget = cameFrom;
                }
            }
            */

            //YES we are able to get here from this position
            CameFromPoint = target;
            SetCurrentCameraLayer(target.GetCameraLayer());
            transform.parent = null;
            tmpTarget = target;
            transform.LerpTransform(this, target.PrefCamTransformPosRot.position, time);
            StartCoroutine(MoveCoroutine(time, target.PrefCamTransformPosRot));
            //when we arrive we need to add this target to the visitedTargets and set the value to true
            //visitedTargets.Add(target.transform, true);

            GameManager.Instance.CurrentlyViewing = target.gameObject.name;
        }

        public void MoveToLastTarget(float time)
        {
            transform.parent = null;
            if(GetCurrentCameraLayer().Equals(CameraLayer.VantagePoint))
            {
                CameFromPoint = StartPoint;
            }
            //SetCurrentCameraLayer(CameFromPoint.GetCameraLayer());
            transform.LerpTransform(this, CameFromPoint.CameFrom.PrefCamTransformPosRot.position, time);
            StartCoroutine(MoveCoroutine(5, CameFromPoint.CameFrom.PrefCamTransformPosRot));
        }

        IEnumerator MoveCoroutine(float timeToMove, Transform nextTarget)
        {
            float currentTime = 0;
            
            while (currentTime < timeToMove)
            {
                float currentDistance = currentTime / timeToMove;
                //transform.position = Vector3.Lerp(currentTarget.position, nextTarget.position, currentDistance);
                //transform.LerpTransform(this, nextTarget.position, currentDistance);
                transform.rotation = Quaternion.Slerp(CameFromPoint.PrefCamTransformPosRot.rotation, nextTarget.rotation, currentDistance);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}