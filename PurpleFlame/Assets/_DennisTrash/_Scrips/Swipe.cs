using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class Swipe : MonoBehaviour
    {
        public Vector2 SwipeDelta { get { return swipeDelta; } }
        public bool SwipeUp { get { return swipeUp; } }
        public bool SwipeRight { get { return swipeRight; } }
        public bool SwipeLeft { get { return swipeLeft; } }
        public bool SwipeDown { get { return swipeDown; } }

        private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
        private bool isDragging = false;
        private Vector2 startTouch, swipeDelta;

        private void Update()
        {
            tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

            MobileInput();

            swipeDelta = Vector2.zero;
            if (isDragging)
            {
                if (Input.touches.Length > 0) { swipeDelta = Input.touches[0].position - startTouch; }
            }

            SwipeDirection();
        }

        public void MobileInput()
        {
            int i = 0;
            while (i < Input.touchCount)
            {
                Touch touch = Input.GetTouch(i);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                if (touch.phase == TouchPhase.Began)
                {
                    isDragging = true;
                    tap = true;
                    startTouch = Input.touches[0].position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isDragging = false;
                    Reset();
                }
                ++i;
            }
        }

        private void SwipeDirection()
        {
            if (swipeDelta.magnitude > 180)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                //Left or Right
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    if (x < 0) { swipeLeft = true; }
                    else { swipeRight = true; }
                }
                //Up or Down
                else
                {
                    if (y < 0) { swipeDown = true; }
                    else { swipeUp = true; }
                }

                Reset();
            }
        }

        private void Reset()
        {
            startTouch = swipeDelta = Vector2.zero;
            isDragging = false;
        }
    }
}