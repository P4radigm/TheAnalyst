using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class LetterBoxObject : MonoBehaviour
    {
        public int aPart;
        [HideInInspector] public bool inPosition = false;

        private Collider collider;
        private Vector3 startPos;

        private void Start()
        {
            collider = GetComponent<Collider>();
            startPos = this.transform.position;
        }

        public void DisableCollider(bool state)
        {
            collider.enabled = !state;
        }

        public void UpdatePosition(Vector3 hitPos)
        {
            this.transform.position = hitPos;
        }

        public void AddCurrentEndPosition(Transform endPos)
        {
            this.transform.position = endPos.position;
            this.transform.rotation = endPos.rotation;
        }

        public void ResetPosition()
        {
            this.transform.position = startPos;
        }
    }
}
