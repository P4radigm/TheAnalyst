using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class CameraLook : MonoBehaviour
    {
        public Transform Target;

        private void Update()
        {
            if (Target != null)
            {
                Vector3 direction = Target.position - transform.position;
                direction.y = 0f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), Time.time * 4f);
                transform.LookAt(Target); 
             }
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }
    }
}
