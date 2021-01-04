using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using BasHelpers;

namespace TouchBehaviours
{
    public class DragBase : TouchableEntity
    {
        // objects where if the camera is focussed on it, you can interact with this object
        //public GoTo[] canInteractFrom
        public GameObject[] CanInteractFrom;
        public bool touchWhenNotVisible = false;
        bool touchevents = false;
        bool renderVisible = false;
        // protected bool touchUpdate;

        // is the object being rendered (inside view, or outside view but casting shadows)
        protected bool RenderVisisble
        {
            get { return renderVisible; }
        }

        protected override void Start()
        {

            GameManager = GameManager.Instance;

        }

        protected override void OnEnable()
        {
            GameManager = GameManager.Instance;
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

        protected override void OnFingerDown(LeanFinger finger)
        {           
            if (CanInteract())
            {
                if (TouchHelper.TouchesObjectInLayers(finger, gameObject, GameManager.TouchableLayers))
                {
                    base.OnFingerDown(finger);
                }
            }
        }


        public bool CanInteract()
        {            
            if (this.enabled && !GameManager.IsPaused ) //!GameManager.cameraScript.isControlDisabled && !GameManager.inventoryManager.IsZoomed)
            {
                
                string name = GameManager.CurrentlyViewing;
                for (int i = 0; i < CanInteractFrom.Length; i++)
                {
                    if (CanInteractFrom[i] != null && name == CanInteractFrom[i].name)
                        return true;
                }
                return true;
            }
            return true;
        }
    }
}
