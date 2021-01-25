using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class SetChessPieceOnBoard : MonoBehaviour
    {
        [SerializeField] private Collider chessPiece;
        [SerializeField] private Collider chessBoardCollider;

        private void Start()
        {
            chessBoardCollider.enabled = false;
            chessPiece.enabled = false;
        }

        public void SetOnBoard()
        {
            chessPiece.enabled = true;
            chessBoardCollider.enabled = true;
        }
    }
}