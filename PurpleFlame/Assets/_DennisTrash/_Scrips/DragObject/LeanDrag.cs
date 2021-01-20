using UnityEngine;
using System.Collections.Generic;
using Lean;
using System.Collections;

namespace PurpleFlame
{
	public abstract class LeanDrag : Touchable
	{

		// objects where if the camera is focussed on it, you can interact with this object
		//public GoTo[] canInteractFrom; 
		public bool touchWhenNotVisible = false;
		bool touchevents = false;
		bool renderVisible = false;
		// protected bool touchUpdate;

		// is the object being rendered (inside view, or outside view but casting shadows)
		protected bool RenderVisisble
		{
			get { return renderVisible; }
		}

		protected override void OnEnable()
		{
			Renderer render = GetComponent<Renderer>();
			// if renderer == nul OnBecameVisible won't be triggered
			if ((renderVisible || touchWhenNotVisible || render == null) && !touchevents)
			{
				touchevents = true;
				renderVisible = true;
				// register tap events
				base.OnEnable();
			}
		}

		protected override void OnDisable()
		{
			renderVisible = false;
			if (touchevents)
			{
				touchevents = false;
				base.OnDisable();
			}
		}

		//enable touch registration when it becomes visible instead of when enabled
		void OnBecameVisible()
		{
			renderVisible = true;
			if (this.enabled && !touchevents)
			{
				touchevents = true;
				base.OnEnable();
			}
		}

		//disable touch registration if this is no longer being rendered (or used to render shadows)
		void OnBecameInvisible()
		{
			renderVisible = false;
			if (touchevents && !touchWhenNotVisible)
			{
				touchevents = false;
				base.OnDisable();
			}
		}

		protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
		{
			base.OnFingerDown(finger);
		}


		public bool CanInteract()
		{
			return false;
		}
	}
}
