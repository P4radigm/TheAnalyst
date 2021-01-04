using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasHelpers;

namespace TouchBehaviours
{
    public class AvatarSecondDesk : MonoBehaviour
    {
        private Vector3 StartPosition;
        private Vector3 nextPosition;
        private Vector3 oldPos;
        private Vector3 targetVector;
        public float InterpolationSpeed = 5f;
        private bool activateAvatar = false;

        private List<Vector3> wayPoints = new List<Vector3>();
        private int index = 0;
        private void Start()
        {

        }

        public void UpdateNextPosition(Vector3 position)
        {
            wayPoints.Add(position);
            activateAvatar = true;
            
            //StartCoroutine(MoveTowardsNextPosition(wayPoints));
        }

        private void Update()
        {
            if (!activateAvatar) return;
            if (index <= wayPoints.Count - 1)
            {
                if (Vector3.Distance(this.transform.position.ToZeroZ(), wayPoints[index].ToZeroZ()) < 0.01)
                {
                    index++;
                    oldPos = this.transform.position;
                }
                else
                {
                    transform.LerpTransform(this, wayPoints[index], InterpolationSpeed);
                }
            }
          
        }

        private IEnumerator MoveTowardsNextPosition(List<Vector3> position)
        {
            yield return new WaitForFixedUpdate();
            if (position.Count < 1) yield return null;
            transform.LerpTransform(this, position[index], InterpolationSpeed);
            if(Vector3.Distance(this.transform.position.ToZeroZ(), position[index].ToZeroZ()) < 0.01)
            {
                index++;
            }
        }
    }
}