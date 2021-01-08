using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class CupBoardManager : LeanDrag
    {
        [Header("Preference values")]
        [SerializeField] private float yPosWeights;

        [Header("Attributes")]
        [SerializeField] private Weight[] weights;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask layerMaskWheel;
        [SerializeField] private Collider touchInputCollider;
        [SerializeField] private GameObject[] weightPosition;
        [SerializeField] private Animator animChess;

        private Weight selectedWeight;
        private Vector3 position;
        private int correctWeights;
        private bool untouchable;

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);
            if (untouchable) { return; }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<Weight>())
                {
                    selectedWeight = hit.collider.gameObject.GetComponent<Weight>();
                    selectedWeight.DisableCollider(true);
                    selectedWeight.CheckCurrentWeightPos();
                    touchInputCollider.enabled = true;
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            if (untouchable) { return; }
            touchInputCollider.enabled = false;
            if (selectedWeight == null) { return; }
            CheckPosition();
            selectedWeight.DisableCollider(false);
            selectedWeight = null;
        }

        private void Update()
        {
            if(selectedWeight != null) { MoveWeight(); }
        }

        private void MoveWeight()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskWheel))
            {
                position = hit.point;
            }
            //position.y = yPosWeights;
            selectedWeight.UpdatePosition(position);
        }

        private void CheckPosition()
        {
            bool foundPos = false;
            WeightPlacePos weightPlacePos;
            ScalePlacePos scalePlacePos;
            for (int i = 0; i < weightPosition.Length; i++)
            {
                if (weightPosition[i].GetComponent<WeightPlacePos>())
                {
                    weightPlacePos = weightPosition[i].GetComponent<WeightPlacePos>();

                    if (weightPlacePos.placeOccupied != null) { continue; }
                    if (weightPlacePos.disToWeightPos > Vector3.Distance(selectedWeight.transform.position, weightPosition[i].transform.position))
                    {
                        foundPos = true;
                        weightPlacePos.AddWeight(selectedWeight);
                        selectedWeight.AddCurrentWeightPosition(weightPlacePos);
                    }
                }
                if (weightPosition[i].GetComponent<ScalePlacePos>())
                {
                    scalePlacePos = weightPosition[i].GetComponent<ScalePlacePos>();

                    if (scalePlacePos.placeOccupied != null) { continue; }
                    if (scalePlacePos.disToWeightPos > Vector3.Distance(selectedWeight.transform.position, weightPosition[i].transform.position))
                    {
                        foundPos = true;
                        scalePlacePos.AddWeight(selectedWeight);
                        selectedWeight.AddCurrentWeightPosScale(scalePlacePos);
                    }
                }
                
            }
            if (!foundPos) { selectedWeight.ResetPosition(); }
        }

        public void ChangeCorrectWeightNumber(int i)
        {
            correctWeights += i;

            if(correctWeights == 7)
            {
                untouchable = true;
                animChess.SetTrigger("UnlockChessPiece");
            }
        }

        #region Singleton
        private static CupBoardManager instance;

        private void Awake()
        {
            instance = this;
        }

        public static CupBoardManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CupBoardManager();
                }

                return instance;
            }
        }
        #endregion
    }
}