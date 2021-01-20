using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using TouchBehaviours;

namespace PurpleFlame
{
    public class ChessPieceVisible : LeanDrag
    {
        [SerializeField] private LayerMask layer;

        private RaycastHit hit;

        protected sealed override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);
            CheckForChessPiece();
        }

        private void CheckForChessPiece()
        {
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.Instance.TouchableLayers))
            {
                if (!hit.collider.GetComponent<SetChessPieceVisible>()) { return; }
                hit.collider.GetComponent<SetChessPieceVisible>().SetPieceVisible();
            }
        }
    }
}