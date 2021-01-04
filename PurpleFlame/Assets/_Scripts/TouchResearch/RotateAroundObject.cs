using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class RotateAroundObject : MonoBehaviour
    {
        public float RotationSpeed = 20;

        private void OnMouseDrag()
        {
            float rotX = Input.GetAxis("Mouse X") * RotationSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * RotationSpeed * Mathf.Deg2Rad;

            transform.Rotate(Vector3.up, -rotX);
            transform.Rotate(Vector3.right, rotY);
        }
    }
}
