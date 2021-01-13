using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class DoorManager : LeanDrag
    {
        [SerializeField] private float swipeThreshold;
        [SerializeField] private LayerMask layerMask;

        private bool finished;
        private bool interactableHit;
        private float swipeDistance;
        private Door selectedDoor;

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);
            if (finished) { return; }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<Door>())
                {
                    interactableHit = true;
                    selectedDoor = hit.collider.gameObject.GetComponent<Door>();
                    ObjectRotation.Instance.DisableScript(true);
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            if (finished) { return; }
            ResetValues();
        }

        void Update()
        {
            if (interactableHit) { SwipeInput(); }
        }

        private void SwipeInput()
        {
            swipeDistance = touchingFingers[0].ScreenDelta.x;

            if (swipeDistance > swipeThreshold) { selectedDoor.DragDoor(-1); }
            if (swipeDistance < -swipeThreshold) { selectedDoor.DragDoor(1); }
        }

        private void ResetValues()
        {
            interactableHit = false;
            selectedDoor = null;
            swipeDistance = 0;
            ObjectRotation.Instance.DisableScript(false);
        }
    }
}