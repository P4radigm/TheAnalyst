using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class LetterBoxObject : MonoBehaviour
    {
        public int aPart;
        [HideInInspector] public bool inPosition = false;

        public void CurrentPosition(Vector3 currentPosition)
        {
            this.transform.position = new Vector3(currentPosition.x, currentPosition.y, transform.position.z);
        }

        public void OnPosition()
        {
            inPosition = true;
        }   
    }
}
