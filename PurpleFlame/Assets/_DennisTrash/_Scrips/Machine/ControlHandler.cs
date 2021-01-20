using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class ControlHandler : LeanDrag
    {
        [SerializeField] private float angleThreshHold;
        [SerializeField] private float rotateSpeed;
        [Space]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask layerMaskPlane;
        [SerializeField] private GameObject plane;
        [Space]
        [SerializeField] private Animator handleAnimator;
        [SerializeField] private Animator ASectorAnimator;
        [SerializeField] private Animator BControlAnimator;
        [SerializeField] private Animator BSectorAnimator;
        [SerializeField] private Animator CDControlAnimator;
        [SerializeField] private Animator CDSectorAnimator;
        [SerializeField] private float animationSpeedMultiplier;
        private bool Benabled = false;
        private bool CDenabled = false;


        [Header("End Cinematic Stuff")]
        [SerializeField] private GameObject[] enableObjects;
        [SerializeField] private GameObject[] disableObjects;

        private float angleLastFrame = 0;
        private float angle = 0;
        private float rotation = 0;
        private int roundCount;
        private float yRot;
        private float yRotLastFrame;
        private bool interactableHit;
        private Vector3 hitNormal;
        private Vector3 position;

        private void Start()
        {
            handleAnimator.enabled = false;
            ASectorAnimator.enabled = false;
            BControlAnimator.enabled = false;
            BSectorAnimator.enabled = false;
            CDControlAnimator.enabled = false;
            CDSectorAnimator.enabled = false;

            for (int i = 0; i < enableObjects.Length; i++)
            {
                enableObjects[i].SetActive(false);
            }
            for (int i = 0; i < disableObjects.Length; i++)
            {
                disableObjects[i].SetActive(true);
            }
        }

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<ControlHandler>())
                {
                    interactableHit = true;
                    plane.GetComponent<Collider>().enabled = true;
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            interactableHit = false;
            plane.GetComponent<Collider>().enabled = false;
        }

        private void Update()
        {
            if (interactableHit) { RotateInput(); }
        }

        private void RotateInput()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskPlane))
            {
                position = hit.point;
                hitNormal = hit.normal;
            }
            UpdateDiskPos(position);
        }

        //private void UpdateDiskPos(Vector3 hitPos, Vector3 hitNormal)
        //{
        //    Vector3 mouseDir = hitPos - this.transform.position;
        //    var angle = Vector3.SignedAngle(this.transform.forward, mouseDir, transform.up);
        //    if (Mathf.Abs(angle) > angleThreshHold)
        //    {
        //        yRotLastFrame = transform.eulerAngles.y;
        //        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(mouseDir, hitNormal), rotateSpeed * Time.deltaTime);
        //        yRot = transform.eulerAngles.y;
        //    }

        //    if (yRot - yRotLastFrame > 0 && Mathf.Abs(angle) > angleThreshHold) 
        //    {
        //        roundCount++;
        //        if(roundCount > 350) { StartCinematic(); }
        //    }
        //}

        public void UpdateDiskPos(Vector3 hitPos)
        {
            Vector3 dirVec = hitPos - plane.transform.position;

            if (dirVec.magnitude < 0.02f) { return; }

            dirVec = dirVec.normalized;

            Debug.DrawLine(plane.transform.position, plane.transform.position + plane.transform.InverseTransformDirection(dirVec), Color.red);
            Debug.DrawLine(plane.transform.position, plane.transform.position + plane.transform.up, Color.green);

            Vector3 localDirVec = plane.transform.InverseTransformDirection(dirVec);

            float _angle = Mathf.Atan2(localDirVec.z, localDirVec.y) * Mathf.Rad2Deg;

            angle = _angle;

            transform.localRotation = Quaternion.Euler(-_angle, 0, 0);

            float _difference = angle - angleLastFrame;

            if (Mathf.Abs(_difference) < 200)
            {
                //typeWriterUI.UpdatePaper(_difference);
                rotation += _difference;
                float _animControl = _difference * -animationSpeedMultiplier;

                handleAnimator.Update(_animControl);
                ASectorAnimator.Update(_animControl);
                
                if (Benabled)
                {
                    BControlAnimator.Update(_animControl);
                    BSectorAnimator.Update(_animControl);
                }

                if (CDenabled)
                {
                    CDControlAnimator.Update(_animControl);
                    CDSectorAnimator.Update(_animControl);
                }

                if(rotation > 720)
                {
                    Benabled = true;

                    if(rotation > 1440)
                    {
                        CDenabled = true;
                        if(rotation > 720*3)
                        {
                            StartCinematic();
                        }
                    }
                    else
                    {
                        CDenabled = false;
                    }
                }
                else
                {
                    Benabled = false;
                }

                if(rotation < 0) { rotation = 0; }
            }

            angleLastFrame = angle;
        }

        private void StartCinematic()
        {
            for (int i = 0; i < enableObjects.Length; i++)
            {
                enableObjects[i].SetActive(true);
            }
            for (int i = 0; i < disableObjects.Length; i++)
            {
                disableObjects[i].SetActive(false);
            }
        }
    }
}