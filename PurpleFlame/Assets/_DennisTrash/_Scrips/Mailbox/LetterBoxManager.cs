using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchBehaviours;

namespace Dennis
{
    public class LetterBoxManager : LeanDrag
    {

        [Header("Preference values")]
        public float disToEndPos;

        [Header("Attributes")]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask layerMaskField;
        [SerializeField] private Collider touchInputCollider;
        [SerializeField] private Transform endPosition;
        [SerializeField] private Transform[] endPartPosition;

        private LetterBoxObject selectedObject;
        private Vector3 position;
        private int correctParts = 0;
        private bool untouchable;

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);
            if (untouchable) { return; }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<LetterBoxObject>() && !hit.collider.gameObject.GetComponent<LetterBoxObject>().inPosition)
                {
                    selectedObject = hit.collider.gameObject.GetComponent<LetterBoxObject>();
                    selectedObject.DisableCollider(true);
                    touchInputCollider.enabled = true;
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            if (untouchable) { return; }
            
            touchInputCollider.enabled = false;

            if (selectedObject == null) { return; }
            if (selectedObject.inPosition) { return; }

            CheckPosition();
            DisableObject();
        }

        private void Update()
        {
            if (selectedObject != null) { MoveObject(); }
        }

        private void MoveObject()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskField))
            {
                position = hit.point;
            }
            selectedObject.UpdatePosition(position);
        }

        
        private void CheckPosition()
        {
            bool foundPos = false;
            if (disToEndPos > Vector3.Distance(selectedObject.transform.position, endPosition.position))
            {
                foundPos = true;
                selectedObject.AddCurrentEndPosition(endPartPosition[selectedObject.aPart]);
                selectedObject.inPosition = true;
                AddCorrectObjectNumber();
            }
            if (!foundPos) { selectedObject.ResetPosition(); }
        }

        public void AddCorrectObjectNumber()
        {
            correctParts++;

            //finished
            if (correctParts == 3)
            {
                untouchable = true;
                DisableObject();
                Debug.Log("Puzzel Done");
                GameManager.Instance.PuzzleDone(GameState.Act1);
            }
        }

        private void DisableObject()
        {
            if(selectedObject == null) { return; }
            selectedObject.DisableCollider(false);
            selectedObject = null;
        }
    }
}