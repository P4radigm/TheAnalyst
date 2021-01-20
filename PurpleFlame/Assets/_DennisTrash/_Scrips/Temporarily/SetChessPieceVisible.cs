using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class SetChessPieceVisible : MonoBehaviour
    {
        [SerializeField] private GameObject chessPiece;

        private void Start()
        {
            chessPiece.SetActive(false);
        }

        public void SetPieceVisible()
        {
            chessPiece.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}