using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasHelpers;
using Lean.Touch;

namespace TouchBehaviours
{
    public class DragObjectPhysics : DragBase
    {
        public Transform EndLocation;
        public Vector3 StartPosition, EndPosition;
        public DragType DragType;
        public Vector2 MinMaxClamps;
        public float InterpolationSpeed = 5f;

        private Vector3 mOffset;

        private float mZCoord;

        private float perc;

        private bool flag = false;

        private Vector3 hitPosition;
        private Vector3 currentPosition;
        private Vector3 targetPosition;
        private Vector3 camPosition;

        //Doel: Door middel van finger positie en drag willen we lerpen tussen 2 posities, 
        //de waarde hier tussen is wordt bepaald door waar je je finger houd op het scherm

        protected override void Start()
        {
            base.Start();
            StartPosition = transform.position;
            EndPosition = EndLocation.position;
        }

        private void Update()
        {

            if(touchingFingers.Count>0)
            {
                hitPosition = Input.mousePosition;
                camPosition = transform.position;

                switch(DragType)
                {
                    case DragType.DeskDrawer:
                        transform.position = Vector3.MoveTowards(transform.position, GetMouseWorldPosZAxis(), InterpolationSpeed * Time.deltaTime);

                        if (transform.position.z > EndPosition.z)
                            transform.position = EndPosition;
                        else if (transform.position.z < StartPosition.z)
                            transform.position = StartPosition;
                        break;
                    case DragType.Handle:
                        transform.position = Vector3.MoveTowards(transform.position, GetMouseWorldPosYAxis(), InterpolationSpeed * Time.deltaTime);

                        if (transform.localPosition.y > EndPosition.y)
                            transform.localPosition = EndPosition;
                        else if (transform.localPosition.y < StartPosition.y)
                            transform.localPosition = StartPosition;
                        break;
                    case DragType.MailBoxDrawer:
                        transform.position = Vector3.MoveTowards(transform.position, GetMouseWorlPosXAxis(), InterpolationSpeed * Time.deltaTime);

                        if (transform.position.x > EndPosition.x)
                            transform.position = EndPosition;
                        else if (transform.position.x < StartPosition.x)
                            transform.position = StartPosition;
                        break;
                }
            }
        }

        private void OnMouseDown()
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            //mOffset = gameObject.transform.position - GetMouseWorldPos();

            //transform.position = Vector3.Lerp(transform.position, GetMouseWorldPos(), InterpolationSpeed * Time.deltaTime);

        }

        protected override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);
        }

        protected override void OnFingerUp(LeanFinger finger)
        {
            /*
            flag = true;
            currentPosition = Input.mousePosition;
            if(flag)
            {
                //When distance to start position is closer than endposition
                if(Vector3.Distance(EndPosition, transform.position) > Vector3.Distance(StartPosition, transform.position))
                {
                    targetPosition = StartPosition;
                }
                else
                {   
                    targetPosition = EndPosition;
                }
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, InterpolationSpeed * Time.deltaTime);
                //keep draging a bit
                if (transform.localPosition == targetPosition)
                {
                    StopAllCoroutines();
                    flag = false; // Stop moving yo
                }
            }*/
            //MouseDrag();
            base.OnFingerUp(finger);
        }

        private void MouseDrag()
        {
            Vector3 dir = Camera.main.ScreenToWorldPoint(currentPosition)-Camera.main.ScreenToWorldPoint(hitPosition);
            //Invert dir to that terrain appears to move with the mouse
            dir = dir * -1;
            //Only use z for drawers
            Vector3 newDirection = new Vector3(transform.position.x, transform.position.y, dir.z);

            targetPosition = camPosition + dir;

        }

        private Vector3 GetMouseWorldPosZAxis()
        {
            Vector3 mousePoint = Input.mousePosition;

            mousePoint.z = mZCoord;
            Vector3 newPosition = Vector3.zero;
            newPosition = new Vector3(transform.position.x, transform.position.y, Camera.main.ScreenToWorldPoint(mousePoint).z);
            return newPosition;
        }

        private Vector3 GetMouseWorldPosYAxis()
        {
            Vector3 mousePoint = Input.mousePosition;

            mousePoint.z = mZCoord;
            Vector3 newPosition = Vector3.zero;
            newPosition = new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(mousePoint).y, transform.position.z);
            return newPosition;
        }

        private Vector3 GetMouseWorlPosXAxis()
        {
            Vector3 mousePoint = Input.mousePosition;

            mousePoint.z = mZCoord;
            Vector3 newPosition = Vector3.zero;
            newPosition = new Vector3(Camera.main.ScreenToWorldPoint(mousePoint).x, transform.position.y, transform.position.z);
            return newPosition;
        }

        private void OnMouseDrag()
        {
            //transform.LerpTransform(this, GetMouseWorldPos(), InterpolationSpeed);
        }
    }
}
