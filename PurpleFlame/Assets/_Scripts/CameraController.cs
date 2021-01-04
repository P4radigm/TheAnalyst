using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchBehaviours;
using Lean.Touch;
using System;
using BasHelpers;

[System.Serializable]
public class WayPoint
{
    public InteractableObject From;
    public InteractableObject ToWards;
    public GameObject WayPointPosition;
    public float TransitionTime = 1;
    public bool InBothDirections;
}

public class CameraController : CameraBase
{
    public WayPoint[] WayPoints;
    public InteractableObject FirstStart;
    public GameObject CameraObject;
    public float PinchBackThreshold = 0.8f;
    public float PlayerRotationSpeed = 0.2f;
    public float TransitionTime = 1;
    [HideInInspector]
    public float CurrentTransitionTime;
    public CanvasGroup MymoveBackButtons;

    [HideInInspector]
    public InteractableObject CurrentInteractableObject;

    private int blockingInteractions = 0;  // interactions blocking camera movement

    private bool rotating = false;

    [HideInInspector]
    public bool DidSetupCamera = false;
    //	private float timeRotated;
    //	private float timeToRotate;

    // rotation angle variables for camera movement with fingers
    float verticalTarget;
    float horizontalTarget;
    float horizontalCurrent;
    float verticalCurrent;
    //	float horizontalStart;
    //	float verticalStart;

    //private bool pauzed = true;
    public float pauzeTransitionSpeed;

    public bool isControlDisabled
    {
        get { return disableControl; }
    }

    public string CurrentGoToName
    {
        get { return CurrentInteractableObject.name; }
    }

    // once at the start of the game
    void Start()
    {
        if (!DidSetupCamera && FirstStart != null && CurrentInteractableObject == null)
        {
            StartFromSave(FirstStart);
        }
        disableControl = false;
        StartCoroutine(SetObjects(CurrentInteractableObject));
    }

    public override void OnFingerDown(LeanFinger finger)
    {
        base.OnFingerDown(finger);
    }

    public override void OnFingerUp(LeanFinger finger)
    {
        base.OnFingerUp(finger);
    }

    public override void OnTap(LeanFinger finger)
    {
        base.OnTap(finger);
    }

    //move the camera to another object
    public void ChangeView(GameObject goToObject)
    {
        InteractableObject goTo = null;

        goTo = goToObject.GetComponent<InteractableObject>();


        if (goTo == null)
        {
            /*
            GoToSegment s = goToObject.GetComponent<GoToSegment>();
            if (s != null)
                goTo = s.parent;*/
        }
        if (goTo != null)
            ChangeView(goTo);
    }

    //move the camera to another object
    public void ChangeView(InteractableObject destination, bool animated = true)
    {
        ChangeCameraFocus(destination, animated);
    }

    // move the camera back
    public void MoveBack()
    {
        if (CurrentInteractableObject.Back)
            ChangeView(CurrentInteractableObject.Back);
    }

    /*	private void RestartRotation()
        {
            //horizontalStart = horizontalCurrent;
            //verticalStart = verticalCurrent;
            //timeToRotate = transitionTime;
            //timeRotated = 0;
        } */

    // Update is called once per frame
    void Update()
    {
        // if the camera is allowed to move
        if (!disableControl && !GameManager.Instance.IsPaused)
        {
            // how much the finger has moved accross the screen
            Vector2 dragDelta = Vector2.zero;
            if (CanMove())
            {
                /*
                dragDelta = new Vector2(LeanTouch.;
                if (CurrentInteractableObject.inverseMovement)
                    dragDelta = -dragDelta;*/
            }
            /*
            // if this is the first frame finger #1 is down, reset rotation variables
            if (firstFingerFrame && firstFinger != null)
            {
                firstFingerFrame = false;
                if (dragDelta == Vector2.zero) // stop rotating when a new 1st finger is held down
                    rotating = false;
                //RestartRotation();
            }*/

            // go back to previous object
            if (CurrentInteractableObject.allowPinchBack && CurrentInteractableObject.Back != null)
            {
                MoveBack();
                return; // no need to move camera anymore
            }
            /*
            // dragging a finger accross the screen
            if (firstFinger != null && dragDelta != Vector2.zero)
            {
                /* if (!rotating) // when finger is held down so long, the movement has stopped
                 {
                     RestartRotation();
                 } 
                Vector2 rotationChange = new Vector3(-dragDelta.y * PlayerRotationSpeed, dragDelta.x * PlayerRotationSpeed);

                horizontalTarget += rotationChange.y;
                // limit horizontal rotation (if needed)
                if (CurrentInteractableObject.limitHorizontalRotation)
                {
                    if (horizontalTarget > CurrentInteractableObject.MaxRotateLeftTo) // left past max
                        horizontalTarget = CurrentInteractableObject.MaxRotateLeftTo;
                    else if (horizontalTarget < -CurrentInteractableObject.MaxRotateRightTo) // right past max
                        horizontalTarget = -CurrentInteractableObject.MaxRotateRightTo;
                }

                verticalTarget += rotationChange.x;
                // always limit vertical rotation
                if (verticalTarget > CurrentInteractableObject.MaxRotateUpTo) //up past max
                    verticalTarget = CurrentInteractableObject.MaxRotateUpTo;
                else if (verticalTarget < -CurrentInteractableObject.MaxRotateDownTo) // down past max
                    verticalTarget = -CurrentInteractableObject.MaxRotateDownTo;


                //enable movement rotation interpolation
                rotating = true;
            }*/
        }
        // make sure the camera moves
        InterpolateMovement();
    }

    // rotate the camera according to the players finger movement, interpolated for a smooth effect
    void InterpolateMovement()
    {
        if (rotating)
        {
            rotating = false;

            //if quaternions are multiplied in a different order, you get unwanted rotation around z axis
            transform.rotation *= Quaternion.Euler(verticalTarget - verticalCurrent, 0, 0);
            transform.rotation = Quaternion.AngleAxis(horizontalTarget - horizontalCurrent, CurrentInteractableObject.up) * transform.rotation;

            horizontalCurrent = horizontalTarget;
            verticalCurrent = verticalTarget;
        }
    }

    //Locate the location to go to and set all variables ready to be on that place and move towards it
    void ChangeCameraFocus(InteractableObject newGoTo, bool animated = true)
    {
        GameManager manager = GameManager.Instance;

        rotating = false; // stop the movement interpolation
        manager.CurrentlyViewing = newGoTo.name;
        Quaternion targetRotation;

        // determine if we have to change the rotation to the target's rotation
        if ((CurrentInteractableObject != null && CurrentInteractableObject.dontRotateBack) &&
            (CurrentInteractableObject.Back != null && newGoTo.gameObject.name == CurrentInteractableObject.Back.name)
            ||
            (newGoTo.dontRotateTo))
        {
            Vector3 temp = newGoTo.finalRotation.eulerAngles;
            temp.y = transform.rotation.eulerAngles.y;
            targetRotation = Quaternion.Euler(temp);
        }
        else
            targetRotation = newGoTo.finalRotation;

        /*
        if (GameManager.Instance.showBackButton)
        {
            if (MymoveBackButtons.gameObject.activeInHierarchy && !newGoTo.allowPinchBack)
            {
                StartCoroutine(manager.FadeGui(false, MymoveBackButtons));
            }
        }
        */
        WayPoint wp = CheckForWaypoint(CurrentInteractableObject, newGoTo);
        CurrentInteractableObject = newGoTo;


        StartCoroutine(SetObjects(newGoTo));

        if (animated)
        {
            // start the interpolation for the camera transition
            if (wp != null)
            {
                CurrentTransitionTime = wp.TransitionTime;
            }
            else
            {
                CurrentTransitionTime = TransitionTime;
            }

            StartCoroutine(TransitionCamera(newGoTo.finalPosition, targetRotation, newGoTo.cameraDistanceTo, wp, () => {
                /*if (GameManager.Instance.showBackButton)
                {
                    if (!MymoveBackButtons.gameObject.activeInHierarchy && newGoTo.allowPinchBack)
                {
                        StartCoroutine(manager.FadeGui(true, MymoveBackButtons));
                    }
                }*/
            }));
        }
        else
        {
            transform.rotation = targetRotation;
            transform.position = newGoTo.finalPosition;
            CameraObject.transform.localPosition = new Vector3(0, 0, -newGoTo.cameraDistanceTo);
            if (!MymoveBackButtons.gameObject.activeInHierarchy && newGoTo.allowPinchBack)
            {
                MymoveBackButtons.alpha = 1;
                MymoveBackButtons.gameObject.SetActive(true);
            }
            else if (MymoveBackButtons.gameObject.activeInHierarchy && !newGoTo.allowPinchBack)
            {
                MymoveBackButtons.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator SetObjects(InteractableObject newGoTo)
    {
        if (newGoTo.delayEnable && newGoTo.disableDelay > 0)
            yield return new WaitForSeconds(newGoTo.disableDelay);
        for (int i = 0; i < newGoTo.enableWhenHere.Length; i++)
        {
            if (newGoTo.enableWhenHere[i] != null)
            {
                newGoTo.enableWhenHere[i].SetActive(true);
            }
        }
        if (!newGoTo.delayEnable && newGoTo.disableDelay > 0)
            yield return new WaitForSeconds(newGoTo.disableDelay);
        for (int i = 0; i < newGoTo.disableWhenHere.Length; i++)
        {
            if (newGoTo.disableWhenHere[i] != null)
            {
                newGoTo.disableWhenHere[i].SetActive(false);
            }
        }
        yield return null;
    }

    private IEnumerator TransitionCamera(Vector3 targetPosition, Quaternion targetRotation, float targetCameraZoom, WayPoint wp = null,Action  OnComplete = null)
    {
        disableControl = true;
        float elapsed = 0;
        float startCamZoom = -CameraObject.transform.localPosition.z;
        float timeFrac = 0;
        float smoothStep = 0;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // check if any rotation about z axis is happening
        bool gimbal = Mathf.Abs(transform.eulerAngles.z) > 90;
        bool oncomplete = false;

        // perform the interpolations in this loop
        while (elapsed < CurrentTransitionTime)
        {


            elapsed += Time.deltaTime;

            timeFrac = elapsed / CurrentTransitionTime;
            if (!oncomplete && timeFrac > 0.75f)
            {
                oncomplete = true;
                if (OnComplete != null)
                {
                    OnComplete();
                }
            }

            smoothStep = Mathf.SmoothStep(0, 1, timeFrac);

            // apply rotation

            //ignore z axis, except when you rotate  vertically past the up axis (gimbal lock)
            if (gimbal)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothStep);
            }
            else // ignoring z axis avoids tilt around z, and feels more natural
            {
                Vector3 rotation = new Vector3(Mathf.LerpAngle(startRotation.eulerAngles.x, targetRotation.eulerAngles.x, smoothStep),
                                               Mathf.LerpAngle(startRotation.eulerAngles.y, targetRotation.eulerAngles.y, smoothStep),
                                               0);
                transform.rotation = Quaternion.Euler(rotation);
            }
            // apply translation
            if (wp != null) // waypoint
            {
                transform.position = CustomMath.QuadraticBezier(startPosition, wp.WayPointPosition.transform.position, targetPosition, smoothStep);
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, smoothStep);
            }
            // zoom camera
            CameraObject.transform.localPosition = new Vector3(0, 0, -Mathf.SmoothStep(startCamZoom, targetCameraZoom, timeFrac));
            yield return null;
        }

        //ensure the end points have been reached
        if (gimbal)
            transform.rotation = targetRotation;
        else
            transform.rotation = Quaternion.Euler(new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0));
        transform.position = targetPosition;
        CameraObject.transform.localPosition = new Vector3(0, 0, -targetCameraZoom);

        // player is allowed to move again

        disableControl = false;

        horizontalCurrent = 0;
        verticalCurrent = 0;
        verticalTarget = 0;
        horizontalTarget = 0;

        yield return null;
    }


    WayPoint CheckForWaypoint(InteractableObject current, InteractableObject next)
    {
        foreach (WayPoint w in WayPoints)
        {
            //ignore disabled waypoints
            if (w.WayPointPosition.activeInHierarchy)
            {
                if ((current == w.From && next == w.ToWards) || (w.InBothDirections && current == w.ToWards && next == w.From))
                {
                    return w;
                }
            }
        }
        return null;
    }

    public void EnableGoBackButton()
    {
        /*
        if (CurrentInteractableObject != null && CurrentInteractableObject.allowPinchBack)
        {
            StartCoroutine(GameManager.Instance.FadeGui(true, MymoveBackButtons));
        }*/
    }

    public void DisableGoBackButton()
    {/*
        if (MymoveBackButtons.gameObject.activeSelf)
        {
            StartCoroutine(GameManager.Instance.FadeGui(false, MymoveBackButtons));
        }*/
    }

    // camera can move when no object is being interacted with
    bool CanMove()
    {
        if (blockingInteractions < 0)
            blockingInteractions = 0;
        return blockingInteractions == 0;
    }

    //prepare the right position from a savegame
    public void StartFromSave(InteractableObject startGoto)
    {
        DidSetupCamera = true;
        startGoto.RecalcPositionRotation();
        CurrentInteractableObject = startGoto;
        GameManager.Instance.CurrentlyViewing = startGoto.name;
        CurrentTransitionTime = TransitionTime;
        transform.position = startGoto.finalPosition;
        transform.rotation = startGoto.finalRotation;
        CameraObject.transform.localPosition = new Vector3(0, 0, -startGoto.cameraDistanceTo);
        MymoveBackButtons.gameObject.SetActive(CurrentInteractableObject.allowPinchBack);
    }

    /* Add/Remove BlockInteraction keep track of interactable objects being touched
		if the player interacts with something, the camera can't move */
    public void AddBlockInteraction()
    {
        blockingInteractions++;
    }

    public void RemoveBlockInteraction()
    {
        blockingInteractions--;
    }

    public void ResetBlockingInteractions()
    {
        blockingInteractions = 0;
    }
}


