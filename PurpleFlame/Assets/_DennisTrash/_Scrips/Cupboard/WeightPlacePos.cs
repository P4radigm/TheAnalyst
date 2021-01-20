using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class WeightPlacePos : MonoBehaviour
    {
        public float disToWeightPos;
        public bool backPedestal;
        public Weight placeOccupied;

        public int heavyness;
        [SerializeField] private Transform weightPos;
        
        private Weight previousWeight;

        public void AddWeight(Weight currentWeight)
        {
            placeOccupied = currentWeight;
            placeOccupied.transform.position = weightPos.position;
            if(heavyness == 0) { SwitchWeights(); }
            if (heavyness != placeOccupied.heavyWeight || heavyness == 0) { return; }
            CupBoardManager.Instance.ChangeCorrectWeightNumber(1);
        }

        private void SwitchWeights()
        {

            if(previousWeight != null)
            {
                previousWeight.AddCurrentWeightPosition(placeOccupied.lastWeightPos, true);
                previousWeight.SetStartPos(placeOccupied.lastPos);
            }
            placeOccupied.lastWeightPos.NewPreviousWeight(previousWeight);
            placeOccupied.SetStartPos(weightPos.position);
        }

        public void RemoveWeight()
        {
            if(placeOccupied == null) { return; }
            if (heavyness == placeOccupied.heavyWeight) { CupBoardManager.Instance.ChangeCorrectWeightNumber(-1); }
            if(heavyness == 0) { previousWeight = placeOccupied; }
            placeOccupied = null;
        }

        public void NewPreviousWeight(Weight newPreviousWeight)
        {
            if (!backPedestal) { return; }
            previousWeight = newPreviousWeight;
        }
    }
}