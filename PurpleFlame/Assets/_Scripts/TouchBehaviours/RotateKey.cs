using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dennis;
using Lean.Touch;

namespace TouchBehaviours
{
    public class RotateKey : LeanDrag
    {
        public float TurnSpeed = 5;
        private float startPosition;
        private bool inLock = false;

        protected override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);
            startPosition = finger.ScreenPosition.x;
            mPrevPos = Input.mousePosition;
        }

        protected override void OnFingerUp(LeanFinger finger)
        {
            base.OnFingerUp(finger);
        }

        Vector3 mPrevPos = Vector3.zero;
        Vector3 mPosDelta = Vector3.zero;

        protected override void OnFingerUpdate(LeanFinger finger)
        {
            base.OnFingerUpdate(finger);

            if (startPosition > finger.ScreenPosition.x)
            {
                transform.Rotate(Vector3.back, TurnSpeed * Time.deltaTime);
            }
            else if (startPosition < finger.ScreenPosition.x)
            {
                transform.Rotate(Vector3.back, -TurnSpeed * Time.deltaTime);
            }
            if(transform.localEulerAngles.z > 180)
            {
                if(inLock)
                {
                    GameManager.Instance.GoToNextScene();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "AdaDoor")
            {
                inLock = true;
            }
        }
    }
}
