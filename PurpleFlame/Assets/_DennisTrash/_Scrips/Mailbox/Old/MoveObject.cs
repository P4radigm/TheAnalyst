using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchBehaviours;

namespace Dennis
{
    public class MoveObject : LeanDrag
    {
        /*
        [SerializeField] private LayerMask layer;
        [SerializeField] private float maxDisToEndPos;
        [SerializeField] private Transform EndPos;
        [SerializeField] private Transform[] aPositions; //short part as last

        private int aCount;
        private int aLongPartCount;
        private bool hitObject;
        private bool puzzelFinished;
        private Vector3 startPosition;
        private RaycastHit hit;
        private LetterBoxObject letterBoxObject;
        private Camera camera;

        private void Start()
        {
            camera = Camera.main;
        }

        private void Update()
        {
            InputTouch();
        }

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);

            if (!hitObject) { return; }

            CheckObjectDestination();

            if(aCount == aPositions.Length)
            {
                Debug.Log("Puzzel Done");
                GameManager.Instance.PuzzleDone(GameState.Act1);
                puzzelFinished = true;
            }

            hitObject = false;
        }

        private void InputTouch()
        {
            if (touchingFingers.Count > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer) && !hitObject) 
                { 
                    letterBoxObject = hit.collider.GetComponent<LetterBoxObject>();
                    if (letterBoxObject == null) return;
                    if (letterBoxObject.inPosition) { return; }

                    hitObject = true;
                    startPosition = letterBoxObject.transform.position;
                }
                if (!hitObject) { return; }
               
                Vector3 touchPosition = GetTouchPosition(touchingFingers[0].ScreenPosition);

                //fix Pos according to Cam position
                float disZ = Mathf.Abs(transform.position.z - letterBoxObject.transform.position.z);
                float i = (disZ * 0.1f);
                float y = transform.position.y * 0.5f;
                float x = transform.position.x * 0.5f;
                touchPosition = new Vector3(touchPosition.x - 0.3f, touchPosition.y - 2.405f, transform.position.z);
                letterBoxObject.CurrentPosition(-touchPosition * 0.8f);
            }
        }

        private void CheckObjectDestination()
        {
            float disToEnd = Vector3.Distance(letterBoxObject.transform.position, EndPos.position);
            if (disToEnd < maxDisToEndPos)
            {
                letterBoxObject.transform.position = aPositions[letterBoxObject.aPart - 1].position;
                letterBoxObject.transform.rotation = aPositions[letterBoxObject.aPart - 1].rotation;

                letterBoxObject.OnPosition();
                aCount++;
            }
            else
            {
                letterBoxObject.CurrentPosition(startPosition);
            }

            letterBoxObject = null;
        }

        private Vector3 GetTouchPosition(Vector2 touchPosition)
        {
            return camera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z));
        }

        public bool GetPuzzelFinished()
        {
            return puzzelFinished;
        }
        */
    }
}