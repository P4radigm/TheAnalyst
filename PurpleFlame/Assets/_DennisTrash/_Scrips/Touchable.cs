using UnityEngine;
using System.Collections.Generic;

namespace PurpleFlame
{
	public abstract class Touchable : MonoBehaviour
	{

		// The fingers that are currently held down on this GameObject
		protected List<Lean.Touch.LeanFinger> touchingFingers = new List<Lean.Touch.LeanFinger>();
		//protected Manager manager;

		protected virtual void Start()
		{
			//manager = Manager.Instance;
		}

		protected virtual void OnEnable()
		{
			// Hook into the OnFingerDown & Up events 
			Lean.Touch.LeanTouch.OnFingerDown -= OnFingerDown; // do -= first to ensure there can be only 1
			Lean.Touch.LeanTouch.OnFingerDown += OnFingerDown;
			Lean.Touch.LeanTouch.OnFingerUp -= OnFingerUp;
			Lean.Touch.LeanTouch.OnFingerUp += OnFingerUp;
            Lean.Touch.LeanTouch.OnFingerUpdate += OnFingerUpdate;
		}

		protected virtual void OnDisable()
		{
			// Unhook the OnFingerDown & Up events
			Lean.Touch.LeanTouch.OnFingerDown -= OnFingerDown;
			Lean.Touch.LeanTouch.OnFingerUp -= OnFingerUp;
            Lean.Touch.LeanTouch.OnFingerUpdate -= OnFingerUpdate;
			//ensure all blocking interactions are removed
			for (int i = 0; i < touchingFingers.Count; i++)
			{
				//manager.cameraScript.RemoveBlockInteraction();
			}
			touchingFingers.Clear();
		}

		// triggered when a finger touches the screen
		protected virtual void OnFingerDown(Lean.Touch.LeanFinger finger)
		{
			touchingFingers.Add(finger);
			//manager.cameraScript.AddBlockInteraction();
		}

		protected virtual void OnFingerUpdate(Lean.Touch.LeanFinger finger)
		{

		}

		// triggered when a finger touch ends
		protected virtual void OnFingerUp(Lean.Touch.LeanFinger finger)
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
