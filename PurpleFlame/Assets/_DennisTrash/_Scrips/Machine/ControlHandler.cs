using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class ControlHandler : LeanDrag
    {
        [SerializeField] private float angleThreshHold;
        [SerializeField] private float rotateSpeed;
        [Space]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask layerMaskPlane;

        [Header("End Cinematic Stuff")]
        [SerializeField] private GameObject[] enableObjects;
        [SerializeField] private GameObject[] disableObjects;

        private int roundCount;
        private float yRot;
        private float yRotLastFrame;
        private bool interactableHit;
        private Vector3 hitNormal;
        private Vector3 position;

        private void Start()
        {
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
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            interactableHit = false;
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
            UpdateDiskPos(position, hitNormal);
        }

        private void UpdateDiskPos(Vector3 hitPos, Vector3 hitNormal)
        {
            Vector3 mouseDir = hitPos - this.transform.position;
            var angle = Vector3.SignedAngle(this.transform.forward, mouseDir, transform.up);
            if (Mathf.Abs(angle) > angleThreshHold)
            {
                yRotLastFrame = transform.eulerAngles.y;
                transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(mouseDir, hitNormal), rotateSpeed * Time.deltaTime);
                yRot = transform.eulerAngles.y;
            }

            if (yRot - yRotLastFrame > 0 && Mathf.Abs(angle) > angleThreshHold) 
            {
                roundCount++;
                if(roundCount > 350) { StartCinematic(); }
            }
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