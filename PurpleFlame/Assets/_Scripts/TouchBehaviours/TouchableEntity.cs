using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

namespace TouchBehaviours
{
    public abstract class TouchableEntity : MonoBehaviour
    {
        //The fingers that are currently held down on this entity 
        protected List<LeanFinger> touchingFingers = new List<LeanFinger>();
        protected GameManager GameManager;
        
        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {
            // Hook into the OnFingerDown & Up events 
            LeanTouch.OnFingerDown -= OnFingerDown; // do -= first to ensure there can be only 1
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerUp -= OnFingerUp;
            LeanTouch.OnFingerUp += OnFingerUp;
        }

        protected virtual void OnDisable()
        {
            // Unhook the OnFingerDown & Up events
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerUp -= OnFingerUp;
            
            //ensure all blocking interactions are removed
            for (int i = 0; i < touchingFingers.Count; i++)
            {
                //manager.cameraScript.RemoveBlockInteraction();
            }
            touchingFingers.Clear();
        }

        // triggered when a finger touches the screen
        protected virtual void OnFingerDown(LeanFinger finger)
        {
            touchingFingers.Add(finger);
            //manager.cameraScript.AddBlockInteraction();
        }



        // triggered when a finger touch ends
        protected virtual void OnFingerUp(LeanFinger finger)
        {
            // Was the current finger lifted from the screen?
            if (touchingFingers.Contains(finger))
            {
                // Unset the current finger
                touchingFingers.Remove(finger);
                //Manager.Instance.cameraScript.RemoveBlockInteraction();
            }
        }
    }
}
