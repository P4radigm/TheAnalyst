using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class SetChessPieceOnBoard : MonoBehaviour
    {
        [SerializeField] private ChessPiece chessPiece;
        [SerializeField] private Collider chessBoardCollider;

        private void Start()
        {
            chessBoardCollider.enabled = false;
        }

        public void SetOnBoard()
        {
            chessPiece.SetOnBoard();
            chessBoardCollider.enabled = true;
        }
    }
}