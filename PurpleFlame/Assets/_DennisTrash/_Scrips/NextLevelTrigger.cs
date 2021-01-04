using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using TouchBehaviours;

namespace Dennis
{
    public class NextLevelTrigger : LeanDrag
    {
        [SerializeField] private LayerMask layer;
        [SerializeField] private MainMenu mainMenu;

        private RaycastHit hit;

        protected sealed override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);
            NextLevel();
        }

        private void NextLevel()
        {
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.Instance.TouchableLayers))
            {
                if (!hit.collider.GetComponent<NextLevelTrigger>()) { return; }
                mainMenu.LoadLevel(1);
            }
        }
    }
}