using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using TouchBehaviours;
using BasHelpers;

public class CameraBase : MonoBehaviour
{
    // variables for (touch) tap input
    LeanFinger firstFinger = null;
    bool firstFingerFrame = false;
    public bool disableControl = true;  // disable player movement when the camera is transitioning

    [HideInInspector] public float lastFingerDownTime = 0;

    /*  touch registration (up/down/tap) */

    public virtual void OnFingerDown(LeanFinger finger)
    {
        if (firstFinger == null)
        {
            firstFinger = finger;
            firstFingerFrame = true;
        }
    }

    public virtual void OnFingerUp(LeanFinger finger)
    {
        if (finger == firstFinger)
        {
            firstFinger = null;
            firstFingerFrame = false;
        }
    }

    public virtual void OnTap(LeanFinger finger)
    {
        if (Time.time - lastFingerDownTime >= 0.55f)
        {

            if (!disableControl && !GameManager.Instance.IsPaused)//!GameManager.Instance.inventoryManager.IsZoomed)
            {

                GameObject touched = TouchHelper.GetTouchedObjectInLayers(finger, GameManager.Instance.TouchableLayers);
                if (touched != null)
                {
                    InteractableObject touchedGoTo = touched.GetComponent<InteractableObject>();
                    if (touchedGoTo != null && touchedGoTo.enabled)
                    {
                        if (touchedGoTo.gameObject.activeInHierarchy)
                            touchedGoTo.MoveToPosition();
                    }
                    else
                    {
                        /*
                        GoToSegment touchedSegment = touched.GetComponent<GoToSegment>();
                        if (touchedSegment != null && touchedSegment.enabled && touchedSegment.gameObject.activeInHierarchy)
                            touchedSegment.MoveToParent();*/
                    }
                }
            }
        }
    }

    void OnEnable()
    {
        // Hook into finger events
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerTap += OnTap;
    }

    void OnDisable()
    {
        // unhook the finger events
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
        LeanTouch.OnFingerTap -= OnTap;
    }
}
