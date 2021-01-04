using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class SwipeManager : MonoBehaviour
    {
        public float MaxSwipeTime;
        public float MinSwipeDistance;

        private float swipeStartTime;
        private float swipeEndTime;
        private float swipeTime;

        private Vector2 startSwipePosition;
        private Vector2 endSwipePosition;
        private float swipeLenght;

        private void Update()
        {
            SwipeChecker();
        }

        public void SwipeChecker()
        {
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    swipeStartTime = Time.time;
                    startSwipePosition = touch.position;
                }
                else if(touch.phase == TouchPhase.Ended)
                {
                    swipeEndTime = Time.time;
                    endSwipePosition = touch.position;
                    swipeTime = swipeEndTime - swipeStartTime;
                    swipeLenght = (endSwipePosition - startSwipePosition).magnitude;
                    if(swipeTime < MaxSwipeTime && swipeLenght > MinSwipeDistance)
                    {
                        SwipeControl();
                    }
                }
            }
        }

        private void SwipeControl()
        {
            Vector2 distance = endSwipePosition - startSwipePosition;
            float xDistance = Mathf.Abs(distance.x);
            float yDistance = Mathf.Abs(distance.y);

            if(xDistance > yDistance)
            {
                if(distance.x>0)
                {
                    //Ik heb een swipe gedaan yeeey
                }
                //if(distance.x>0 && swipeCondition)
                //Here we do swipe code.
            }
        }
    }
}