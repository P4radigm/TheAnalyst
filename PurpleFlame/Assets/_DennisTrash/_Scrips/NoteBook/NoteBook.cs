using Lean.Touch;
using System.Collections.Generic;
using TouchBehaviours;
using UnityEngine;

namespace PurpleFlame
{
    public class NoteBook : LeanDrag
    {
        [SerializeField] private LayerMask layer;

        private List<Letter> letters = new List<Letter>();
        private RaycastHit hit;

        protected sealed override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);
            CheckForLetters();
        }

        private void CheckForLetters()
        {
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.Instance.TouchableLayers))
            {
                if (!hit.collider.GetComponent<Letter>()) { return; }
                Letter letterScript = hit.collider.GetComponent<Letter>();
                letters.Add(letterScript);
                letterScript.PutInNotebook();
            }
        }
    }
}
