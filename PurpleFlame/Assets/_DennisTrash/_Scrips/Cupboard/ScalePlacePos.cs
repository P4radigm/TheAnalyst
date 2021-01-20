using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame 
{
    public class ScalePlacePos : MonoBehaviour
    {
        public float disToWeightPos;
        [HideInInspector] public Weight placeOccupied;

        [SerializeField] private int side; //0=left 1=right;
        [Space]
        [SerializeField] private Scale scaleScript;
        [SerializeField] private Transform weightPos;

        private int currentPlaceOccupied;

        public void AddWeight(Weight currentWeight)
        {
            placeOccupied = currentWeight;
            placeOccupied.transform.position = weightPos.position;
            placeOccupied.transform.parent = weightPos;
            scaleScript.AddWeightToScale(placeOccupied, side);
        }

        public void RemoveWeight()
        {
            scaleScript.RemoveWeight(side);
            placeOccupied.SetParentToMain();
            placeOccupied = null;
        }
    }
}