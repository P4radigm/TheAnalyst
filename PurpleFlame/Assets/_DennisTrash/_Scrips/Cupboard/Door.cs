using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float maxRotateAngle;
        [Range(0, 0.5f)]
        [SerializeField] private float resetSpeed;

        [SerializeField] private Transform door;

        private bool done = false;
        private bool finished = false;
        private Vector3 v3;


        private void Start()
        {
            v3 = door.transform.localEulerAngles;
        }

        private void Update()
        {
            if (!done || finished) { return; }

            v3.y = Mathf.Lerp(v3.y, -maxRotateAngle, resetSpeed);
            door.transform.localEulerAngles = v3;

            if (Mathf.Abs(v3.y) + 2 > Mathf.Abs(maxRotateAngle)) { finished = true; }
        }

        public void DragDoor(int value)
        {
            if (done) { return; }
            v3.y += rotateSpeed * value * Time.deltaTime;
            
            if (CheckDoorAngle(maxRotateAngle))
            {
                door.transform.localEulerAngles = v3;

                if (Mathf.Abs(v3.y) > Mathf.Abs(maxRotateAngle - (maxRotateAngle * 0.5f)))
                {
                    done = true;
                }
                
            }
        }
        private bool CheckDoorAngle(float angle)
        {
            bool value = false;
            if(0 < angle && v3.y <= 0 && v3.y > -angle) { value = true; }
            else if (0 > angle && v3.y >= 0 && v3.y < -angle) { value = true; }
            else { v3.y = 0; }
            return value;
        }
        
    }
}