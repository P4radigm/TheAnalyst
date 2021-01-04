using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dennis;
using Lean.Touch;

namespace TouchBehaviours
{
    public class LineDrawer : LeanDrag
    {
        public LineRenderer lineRenderer;
        private Vector3 mousePosition;
        private Vector3 startPosition;
        private float distance;

        protected override void Start()
        {
            base.Start();
            lineRenderer.positionCount = 2;
        }

        protected override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);
            startPosition = Camera.main.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, 19));
        }

        protected override void OnFingerUp(LeanFinger finger)
        {
            base.OnFingerUp(finger);
        }

        protected override void OnFingerUpdate(LeanFinger finger)
        {
            base.OnFingerUpdate(finger);
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, 19));
            lineRenderer.SetPosition(0, new Vector3(startPosition.x, startPosition.y, 19));
            lineRenderer.SetPosition(1, new Vector3(mousePosition.x, mousePosition.y, 19));


            //if finger lets lose on correspondending point to connect line renderer 
        }
    }
}