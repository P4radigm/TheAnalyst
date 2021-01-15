using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean;
using Lean.Touch;

namespace TouchBehaviours
{
    public enum MovingState { StartSide, EndSide, MovingToStart, MovingToEnd }

    public enum DragType { DeskDrawer, Book, Handle, MailBoxDrawer }

    public enum SwipeDirection { Up, Down, Left, Right };


    public class DragObject : DragBase
    {
        public string SoundClipName = "MB1_Drawer";
        public Transform EndLocation;

        public Vector3 StartPosition, EndPosition;

        public MovingState StartState, CurrentState;

        public SwipeDirection CurrentSwipeDirection, SwipeToEndDirection, SwipeToStartDirection;

        public DragType DragType;
        public float MoveDragThreshold; // how much do you have to swipe before you have effect
        public bool smooth; // slowly accelerate and decelerate instead of linear
        public GameObject[] Contents;
        public Collider[] ContentColliders;
        public float Swipe;
        public float MovementSpeed = 5f;
        public bool DestroyOnOpen = false;

        protected float perc;

        private bool swipeRecognized = false;

        protected void SetSwipeDirection()
        {
            switch (SwipeToEndDirection)
            {
                case SwipeDirection.Down:
                    SwipeToStartDirection = SwipeDirection.Up;
                    break;
                case SwipeDirection.Up:
                    SwipeToStartDirection = SwipeDirection.Down;
                    break;
                case SwipeDirection.Left:
                    SwipeToStartDirection = SwipeDirection.Right;
                    break;
                case SwipeDirection.Right:
                    SwipeToStartDirection = SwipeDirection.Left;
                    break;
            }
        }


        protected override void Start()
        {
            base.Start();
            StartPosition = transform.position;
            EndPosition = EndLocation.position;
        }

        private void Update()
        {
            if(touchingFingers.Count > 0 && !swipeRecognized)
            {
                switch(CurrentSwipeDirection)
                {
                    case SwipeDirection.Right: Swipe += touchingFingers[0].ScreenDelta.x; break;
                    case SwipeDirection.Left: Swipe += -touchingFingers[0].ScreenDelta.x; break;
                    case SwipeDirection.Up: Swipe += touchingFingers[0].ScreenDelta.y; break;
                    case SwipeDirection.Down: Swipe += -touchingFingers[0].ScreenDelta.y; break;
                }

                if(Swipe > MoveDragThreshold)
                {
                    swipeRecognized = true;

                    if(CurrentState == MovingState.StartSide || CurrentState == MovingState.MovingToStart)
                    {
                        PreMove(MovingState.EndSide);
                    }
                    /*
                    else if(CurrentState == MovingState.EndSide || CurrentState == MovingState.MovingToEnd)
                    {
                        PreMove(MovingState.StartSide);
                    }
                    */
                }
            }
        }

        public virtual void PreMove(MovingState TargetState)
        {
            //AudioManager.Instance.Play(SoundClipName);
            if(TargetState == MovingState.StartSide && CurrentState != MovingState.StartSide)
            {
                StopAllCoroutines();
                StartCoroutine(Move(TargetState));

            }
            else if(TargetState == MovingState.EndSide && CurrentState != MovingState.EndSide)
            {
                StopAllCoroutines();
                StartCoroutine(Move(TargetState));

            }
        }

        protected sealed override void OnFingerUp(LeanFinger finger)
        {
            Swipe = 0;
            swipeRecognized = false;
            base.OnFingerUp(finger);
        }

        public virtual IEnumerator Move(MovingState target = MovingState.EndSide)
        {
            Vector3 newStartPosition = Vector3.zero;
            Vector3 newEndPosition = Vector3.zero;

            //When we are halfway through movement, don't reset
            if(CurrentState == MovingState.MovingToStart || CurrentState == MovingState.MovingToEnd)
            {
                perc = 1 - perc;
            }
            else
            {
                perc = 0;

            }

            if(target == MovingState.StartSide)
            {
                newStartPosition = EndPosition;
                newEndPosition = StartPosition;
                CurrentState = MovingState.MovingToStart;
                CurrentSwipeDirection = SwipeToEndDirection;
            }
            else if(target == MovingState.EndSide)
            {
                SetContents(true);
                newStartPosition = StartPosition;
                newEndPosition = EndPosition;
                CurrentState = MovingState.MovingToEnd;
                CurrentSwipeDirection = SwipeToStartDirection;
            }
            else
            {
                StopAllCoroutines();
            }

            float t = MovementSpeed * perc;

            Vector3 newPos;
            while (t < MovementSpeed)
            {
                t += Time.deltaTime;
                perc = t / MovementSpeed;
                if (!smooth)
                {
                    newPos = transform.position = Vector3.Lerp(newStartPosition, newEndPosition, perc);
                }
                else
                {
                    newPos = transform.position = Vector3.Lerp(newStartPosition, newEndPosition, Mathf.SmoothStep(0, 1, perc));
                }

                transform.position = newPos;

                /*
                for (int i = 0; i < CanInteractFrom.Length; i++)
                {
                    if (CanInteractFrom[i] = GameManager.cameraScript.currentGoTo)
                    {
                        manager.cameraScript.gameObject.transform.position = newPos;
                        break;
                    }
                }
                */
                yield return null;
            }

            perc = 1;
            transform.position = newEndPosition;
            CurrentState = target;
            if (target == MovingState.StartSide)
            {
                SetContents(false);
                SetContentColliders(false);
            }
            else
            {
                SetContentColliders(true);
            }

            AfterMovementCheck();
            yield return null;
        }

        public virtual void AfterMovementCheck()
        {
            /*
            if (!aftercheckDone)
            {
                aftercheckDone = true;

                if (!loadedFromSave)
                {
                    if (dragType == DragType.cabinet)
                    {
                        manager.SendAnalyticsTimer(AnalyticsTimerEvent.filecabinetsOpened);
                    }
                    else if (dragType == DragType.leftDeskDrawer)
                    {
                        manager.SendAnalyticsTimer(AnalyticsTimerEvent.bookPuzzleOpenDrawer);
                    }
                    else if (dragType == DragType.grammaphone)
                    {
                        manager.SendAnalyticsTimer(AnalyticsTimerEvent.grammaphoneDrawerOpened);
                    }
                }*/

                if (CurrentState == MovingState.EndSide && DestroyOnOpen)
                {
                    Destroy(this);
                }
         }

        protected void SetContents(bool enabled)
        {
            foreach (GameObject g in Contents)
            {
                if (g != null)
                    g.SetActive(enabled);
            }
        }

        protected void SetContentColliders(bool enabled)
        {
            foreach (Collider c in ContentColliders)
            {
                if (c != null)
                    c.enabled = enabled;
            }
        }
    }
}
