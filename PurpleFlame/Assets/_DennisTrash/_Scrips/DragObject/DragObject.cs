using UnityEngine;
using System.Collections;

//Object can move between two points 
//example: drawer that is closed or open
//input swipe
//output lerp
// 
namespace PurpleFlame
{
    public enum Side
    {
        startSide,
        endSide,
        movingToStart,
        movingToEnd,
    }

    public enum DragType
    {
        deskDrawer,
        cabinet,
        grammaphone,
        leftDeskDrawer,
        book
    }

    public class DragObject : LeanDrag
    {
        protected Vector3 startPos, endPos;

        protected bool loadedFromSave = false;

        public Transform endPosition;
        public Side startState;

        public bool smooth; // slowly accelerate and decelerate instead of linear
        public GameObject[] contents;
        public Collider[] contentColliders;

        [HideInInspector]
        public Side currentState;

        public float timeToMove;
        protected float perc;

        public swipeDirection swipeToEndDirection;
        protected swipeDirection currentSwipeDirection, swipeToStartDirection;
        protected float swipe;

        public float moveDragThreshold; // how much do you have to swipe before you have effect
        private bool SwipeRecognised = false;

        public AudioClip[] moveSound;
        public float moveVolume = 1;
        public float movePitch = 1;
        public bool moveAlong, destroyOnOpen;

        public DragType dragType;

        protected bool aftercheckDone;

        protected override void Start()
        {
            base.Start();

            if (!loadedFromSave)
            {
                SetSwipeDirection();
                currentState = startState;
                startPos = transform.position;
                endPos = endPosition.position;

                if (startState == Side.endSide)
                {
                    SetContents(true);
                    SetContentColliders(true);
                    currentSwipeDirection = swipeToStartDirection;
                    transform.position = endPos;
                }
                else
                {
                    SetContents(false);
                    SetContentColliders(false);
                    currentSwipeDirection = swipeToEndDirection;
                    transform.position = startPos;
                }

            }
        }

        protected void SetSwipeDirection()
        {
            switch (swipeToEndDirection)
            {
                case swipeDirection.Down:
                    swipeToStartDirection = swipeDirection.Up;
                    break;
                case swipeDirection.Up:
                    swipeToStartDirection = swipeDirection.Down;
                    break;
                case swipeDirection.Left:
                    swipeToStartDirection = swipeDirection.Right;
                    break;
                case swipeDirection.Right:
                    swipeToStartDirection = swipeDirection.Left;
                    break;
            }
        }

        // check input
        void Update()
        {
            if (touchingFingers.Count > 0 && SwipeRecognised == false)
            {
                switch (currentSwipeDirection)
                {
                    case swipeDirection.Right: swipe += touchingFingers[0].ScreenDelta.x; break;
                    case swipeDirection.Left: swipe += -touchingFingers[0].ScreenDelta.x; break;
                    case swipeDirection.Up: swipe += touchingFingers[0].ScreenDelta.y; break;
                    case swipeDirection.Down: swipe += -touchingFingers[0].ScreenDelta.y; break;
                }

                if (swipe > moveDragThreshold)
                {
                    SwipeRecognised = true;

                    if (currentState == Side.startSide || currentState == Side.movingToStart)
                    {
                        PreMove(Side.endSide);
                    }
                    else if (currentState == Side.endSide || currentState == Side.movingToEnd)
                    {
                        PreMove(Side.startSide);
                    }
                }
            }
        }

        // final check before calling IEnominator and stop currect one if it's still active
        // public: ex: you trigger a switch -- drawer opens
        public virtual void PreMove(Side TargetState)
        {
            if (TargetState == Side.startSide && currentState != Side.startSide)
            {
                StopAllCoroutines();
                StartCoroutine(Move(TargetState));
            }
            else if (TargetState == Side.endSide && currentState != Side.endSide)
            {
                StopAllCoroutines();
                StartCoroutine(Move(TargetState));
            }
        }

        // reset swipe
        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            swipe = 0;
            SwipeRecognised = false;
            base.OnFingerUp(finger);
        }

        public virtual IEnumerator Move(Side target = Side.endSide)
        {
            Vector3 startPos2 = new Vector3(0, 0, 0);
            Vector3 endPos2 = new Vector3(0, 0, 0);

            // if halfway in movement, don't reset startPos
            if (currentState == Side.movingToStart || currentState == Side.movingToEnd)
            {
                perc = 1 - perc;
            }
            else
            {
                perc = 0;
            }

            if (target == Side.startSide)
            {
                startPos2 = endPos;
                endPos2 = startPos;
                currentState = Side.movingToStart;
                currentSwipeDirection = swipeToEndDirection;
            }
            else if (target == Side.endSide)
            {
                //SetContents(true);
                startPos2 = startPos;
                endPos2 = endPos;
                currentState = Side.movingToEnd;
                currentSwipeDirection = swipeToStartDirection;
            }
            else
            {
                StopAllCoroutines();
            }

            float t = timeToMove * perc;

            Vector3 newPos;
            //Vector3 oldPos;
            while (t < timeToMove)
            {
                t += Time.deltaTime;
                perc = t / timeToMove;
                if (!smooth)
                {
                    newPos = transform.position = Vector3.Lerp(startPos2, endPos2, perc);
                }
                else
                {
                    newPos = transform.position = Vector3.Lerp(startPos2, endPos2, Mathf.SmoothStep(0, 1, perc));
                }

                transform.position = newPos;

                yield return null;
            }

            perc = 1;
            transform.position = endPos2;
            currentState = target;
            if (target == Side.startSide)
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

        //if anything must be done after movement
        //inherited classes must call base at at end of this script instead of front
        public virtual void AfterMovementCheck()
        {
            if (!aftercheckDone)
            {
                aftercheckDone = true;

                if (!loadedFromSave)
                {
                    if (dragType == DragType.cabinet)
                    {
                        //manager.SendAnalyticsTimer(AnalyticsTimerEvent.filecabinetsOpened);
                    }
                    else if (dragType == DragType.leftDeskDrawer)
                    {
                        //manager.SendAnalyticsTimer(AnalyticsTimerEvent.bookPuzzleOpenDrawer);
                    }
                    else if (dragType == DragType.grammaphone)
                    {
                        //manager.SendAnalyticsTimer(AnalyticsTimerEvent.grammaphoneDrawerOpened);
                    }
                }

                if (currentState == Side.endSide && destroyOnOpen)
                {
                    Destroy(this);
                }
            }
        }

        protected void SetContents(bool enabled)
        {
            foreach (GameObject g in contents)
            {
                if (g != null)
                    g.SetActive(enabled);
            }
        }

        protected void SetContentColliders(bool enabled)
        {
            foreach (Collider c in contentColliders)
            {
                if (c != null)
                    c.enabled = enabled;
            }
        }
    }
}