using System.Collections;
using System.Collections.Generic;
using TouchBehaviours;
using UnityEngine;

namespace CameraSystem
{
    public class CameraTouchTarget : MonoBehaviour
    {
        public LayerMask GotoLayer;
        public CameraTouchTarget CameFrom;
        public List<CameraTouchTarget> CanGoTo;
        public List<CameraTouchTarget> CanCameFrom;

        /// <summary>
        /// The preferred transform where the camera should come when moving over here
        /// </summary>
        public Transform PrefCamTransformPosRot;

        /// <summary>
        /// The active lock state
        /// </summary>
        [SerializeField] private LockState currentLockState;

        /// <summary>
        /// Current Layer the Target object is on
        /// </summary>
        [SerializeField] private CameraLayer cameraLayer;

        public LockState GetLockState()
        {
            return currentLockState;
        }

        public CameraLayer GetCameraLayer()
        {
            return cameraLayer;
        }

    }
}
