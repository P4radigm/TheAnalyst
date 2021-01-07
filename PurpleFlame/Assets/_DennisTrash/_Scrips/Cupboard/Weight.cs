using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class Weight : MonoBehaviour
    {
        public int heavyWeight;
        public WeightPlacePos lastWeightPos;
        [HideInInspector] public Vector3 lastPos;

        private Collider collider;
        private WeightPlacePos weightPlacePos;
        private ScalePlacePos scalePlacePos;
        private Transform mainParent;

        private void Start()
        {
            lastWeightPos.AddWeight(this);
            ResetPosition();
            collider = GetComponent<Collider>();
            mainParent = this.transform.parent;
        }

        public void DisableCollider(bool state)
        {
            collider.enabled = !state;
        }

        public void ResetPosition()
        {
            this.transform.position = lastPos;
            if(lastWeightPos.heavyness != 0) { return; }
            lastWeightPos.placeOccupied = this;
        }

        public void UpdatePosition(Vector3 hitPos)
        {
            this.transform.position = hitPos;
        }

        public void CheckCurrentWeightPos()
        {
            if(lastWeightPos != null)
            {
                lastWeightPos.RemoveWeight();
            }

            if(weightPlacePos != null) 
            {
                weightPlacePos.RemoveWeight();
                weightPlacePos = null;
            }
            if(scalePlacePos != null)
            {
                scalePlacePos.RemoveWeight();
                scalePlacePos = null;
            }
        }

        public void AddCurrentWeightPosition(WeightPlacePos currentPos)
        {
            weightPlacePos = currentPos;
            lastWeightPos = currentPos;
        }

        public void AddCurrentWeightPosScale(ScalePlacePos currentPos)
        {
            scalePlacePos = currentPos;
        }

        public void SetParentToMain()
        {
            this.transform.parent = mainParent;
        }

        public void SetStartPos(Vector3 newPos)
        {
            lastPos = newPos;
        }
    }
}