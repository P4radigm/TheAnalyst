using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasHelpers;
using Cinemachine;
using Dennis;
using Lean.Touch;
using BasHelpers;

namespace TouchBehaviours
{
    public class TouchInputManager : LeanDrag
    {
        public LayerMask VantagePoint;
        public LayerMask InspectableObjectsLayer0;
        public LayerMask InspectableObjectsLayer1;
        public LayerMask InspectableObjectsLayer2;

        public CameraTarget CameraFollowTarget;
        public CameraLook CameraLookAtTarget;
        public ObjectRotation ObjectRotation;
        public GameObject CineMachineCam;
        public float LookAtTime = 2f;
        public float CameraOffsetOfInteractable=0;
        public float InterpolationSpeed = 1.5f;

        public float RaycastRange = 100f;
        public float RequiredTabs = 2;
        [SerializeField] private float tabCount = 0;
        [SerializeField] private float tabCooler = 0.5f;
        private GameObject target;
        private GameObject lastTarget;
        private GameObject avatar;

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
                if(hit.transform.gameObject != null && hit.transform.gameObject.HasComponent<AvatarSecondDesk>())
                {
                    avatar = hit.transform.gameObject;
                }

                if(hit.transform.gameObject != null && hit.transform.gameObject.HasComponent<IPuzzle>())
                {
                    target = hit.transform.gameObject;
                    hit.transform.GetComponent<IPuzzle>().ActivatePressEvents();
                }

                if(hit.transform.gameObject !=null && hit.transform.gameObject.HasComponent<AdaDoor>())
                {
                    target = hit.transform.gameObject;
                    target.transform.LerpTransform(this, Vector3.right * 2, 4f);
                    GameManager.Instance.GoToNextScene();
                }

                //When we hitt a collectable we need to collect this
                if (hit.transform.gameObject != null && hit.transform.gameObject.HasComponent<ICollectable>())
                {
                    hit.transform.GetComponent<ICollectable>().Collect();
                }

                if(hit.transform.gameObject !=null && hit.transform.gameObject.HasComponent<ICity>())
                {
                    //GameManager.Instance.SecondDeskManager.AddVisitedPosition(hit.transform.position);
                    //GameManager.Instance.SecondDeskManager.PuzzleUpdate(hit.transform.gameObject.GetComponent<LocationPoint>());
                    //hit.transform.GetComponent<ICity>().ActivateCity();
                }

                //When we hit a inspectable we do some stuff 
                if (hit.transform.gameObject != null && hit.transform.gameObject.HasComponent<Iinspectable>())
                {
                    target = hit.transform.gameObject;

                    if (target != lastTarget)
                    {
                        lastTarget = target;
                    }

                    if (tabCooler > 0 && tabCount >1)
                    {
                        //has dubbletapped

                        DubbleTapedOnInspectable();    

                    }
                    else
                    {
                        //We have tabbed once

                        tabCooler = 0.5f;
                        tabCount += 1;
                        if(tabCount > 1)
                            DubbleTapedOnInspectable();
                    }
                }
            }      
        }

        protected override void OnFingerUpdate(LeanFinger finger)
        {
            base.OnFingerUpdate(finger);
            if(avatar != null)
            {
                Vector3 newAvatarPosition = Camera.main.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, avatar.transform.position.z));
                avatar.transform.position = newAvatarPosition;
            }
        }

        protected override void OnFingerUp(LeanFinger finger)
        {
            base.OnFingerUp(finger);
            if (avatar != null)
                avatar = null;

            if (target != null && target.HasComponent<IPuzzle>())
            { 
                target.GetComponent<IPuzzle>().ActivateReleaseEvents();
                target = null;
            }
        }

        private void Update()
        {
            //Dubble tap cooldown
            if(tabCooler > 0)
            {
                tabCooler -= 1 * Time.deltaTime;
            }
            else
            {
                tabCount = 0;
            }

            if (touchingFingers.Count > 0)
            {
                //This is only used for data that has to be updated every frame 
                //OnFingerUp and OnFingerDown will do the most of the heavy lifting so mostly put code there
            }
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
            if (CameraLookAtTarget.enabled == false)
                CameraLookAtTarget.enabled = true;

            //Call inspect on inspectable object
            Iinspectable targetInspect = target.GetComponent<Iinspectable>();
            targetInspect.Inspect();

            //Give target for lookat
            CameraLookAtTarget.SetTarget(CameraFollowTarget.transform);

            lastTarget = null;

            //Set the currentlyt viewing to our currently inspected inspectable object ( this is for other scripts )
            GameManager.Instance.CurrentlyViewing = target.name;

            //Get the target his preferred cam position we want to be moving to
            Transform targetPrefferedCamPosition = targetInspect.GetPrefCamPosition();

            //Move towards new target preferred cam position
            if (targetPrefferedCamPosition != null)
                CameraFollowTarget.MoveToNewTarget(targetPrefferedCamPosition, InterpolationSpeed, CameraOffsetOfInteractable);

            CameraFollowTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

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
        /// Method for UI elements 
        /// </summary>
        public void BackToStart()
        {
            StartCoroutine(DissableLookat());
        }

        private IEnumerator HackerMan()
        {
            yield return new WaitForSeconds(5f);
            ObjectRotation.enabled = true;
            CameraLookAtTarget.enabled = true;
        }

        /// <summary>
        /// Dissables the lookat and enables the ObjectRotation of camera using swipe
        /// </summary>
        /// <returns></returns>
        private IEnumerator DissableLookat()
        {
            yield return new WaitForSeconds(LookAtTime);
            ObjectRotation.enabled = true;
            CameraLookAtTarget.enabled = false;
            CameraFollowTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        /// <summary>
        /// Gets the ScreenToWorlPoint touchposition
        /// </summary>
        /// <param name="touchPosition"></param>
        /// <returns></returns>
        private Vector2 GetTouchPosition(Vector2 touchPosition)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z));
        }
    }
}
