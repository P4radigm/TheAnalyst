using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class Key : LeanDrag
    {
        [SerializeField] private float angleThreshHold;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private int rotateCount;
        [Space]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask layerMaskPlane;
        [SerializeField] private GameObject layerMaskPlaneObj;
        [SerializeField] private Vector3 rotateDirection;

        [Header("End Cinematic Stuff")]
        [SerializeField] private GameObject[] enableObjects;
        [SerializeField] private GameObject[] disableObjects;

        private int roundCount;
        private float angleLastFrame = 0;
        private float angle = 0;
        private bool interactableHit;
        private bool finished;
        private Vector3 position;
        private Collider collider;
        private Collider targetCollider;

        private void Start()
        {
            collider = GetComponent<Collider>();
            targetCollider = layerMaskPlaneObj.GetComponent<Collider>();
            targetCollider.enabled = false;

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
            if (finished) { return; }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<ControlHandler>())
                {
                    interactableHit = true;
                    collider.enabled = false;
                    targetCollider.enabled = true;
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            if (finished) { return; }
            if (interactableHit == false) { return; }

            DisableInteractable();
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
            }
            UpdateRotation(position);
        }

        private void UpdateRotation(Vector3 hitPos)
        {
            Vector3 dirVec = hitPos - layerMaskPlaneObj.transform.position;
            if (dirVec.magnitude < 0.02f) { return; }

            Debug.DrawLine(layerMaskPlaneObj.transform.position, layerMaskPlaneObj.transform.position + layerMaskPlaneObj.transform.InverseTransformDirection(dirVec), Color.red);
            Debug.DrawLine(layerMaskPlaneObj.transform.position, layerMaskPlaneObj.transform.position + layerMaskPlaneObj.transform.up, Color.green);

            Vector3 localDirVec = layerMaskPlaneObj.transform.InverseTransformDirection(dirVec);
            float _angle = Mathf.Atan2(localDirVec.x, localDirVec.y) * Mathf.Rad2Deg;
            angle = _angle;
            transform.localRotation = Quaternion.Euler(_angle * rotateDirection.x, _angle * rotateDirection.y, _angle * rotateDirection.z);
            float _difference = angle - angleLastFrame;

            if (Mathf.Abs(_difference) > angleThreshHold)
            {
                roundCount++;
                if (roundCount > rotateCount) { StartCinematic(); }
            }

            angleLastFrame = angle;
        }

        private void StartCinematic()
        {
            finished = true;
            DisableInteractable();

            for (int i = 0; i < enableObjects.Length; i++)
            {
                enableObjects[i].SetActive(true);
            }
            for (int i = 0; i < disableObjects.Length; i++)
            {
                disableObjects[i].SetActive(false);
            }
        }

        private void DisableInteractable()
        {
            interactableHit = false;
            collider.enabled = true;
            targetCollider.enabled = false;
        }
    
    }
}