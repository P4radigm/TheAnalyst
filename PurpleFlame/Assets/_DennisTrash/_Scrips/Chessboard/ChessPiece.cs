using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public enum ChessPieceEnum
    {
        None,
        King,
        Bishop,
        Knight,
        Pawn
    }

    public class ChessPiece : Chessman
    {

        public Vector2 startPosition;
        public ChessPieceEnum chessPiece;

        [HideInInspector] public bool unMoved = true;
        [HideInInspector] public bool onBoard = false;

        private Collider collider;

        private void Start()
        {
            collider = GetComponent<Collider>();
        }

        public void ShowActions(bool[,] t)
        {
            switch (chessPiece)
            {
                case ChessPieceEnum.None:
                    break;
                case ChessPieceEnum.King:
                    KingActions(t);
                    break;
                case ChessPieceEnum.Bishop:
                    BishopActions(t);
                    break;
                case ChessPieceEnum.Knight:
                    KnightActions(t);
                    break;
                case ChessPieceEnum.Pawn:
                    PawnActions(t);
                    break;
                default:
                    break;
            }
        }

        public void SetOnBoard()
        {
            Chessboard.Instance.SetOnBoard(this);
            collider.enabled = true;
        }
        public override bool[,] PossibleMove()
        {
            bool[,] t = new bool[9, 9];

            if (Chessboard.Instance.ClickedTile(transform.position.x, transform.position.z) != startPosition)
            {
                unMoved = false;
                BackToStartPos(t);
            }
            else
            {
                unMoved = true;
                ShowActions(t);
            }
            return t;
        }

        private void BackToStartPos(bool[,] t)
        {
            t[(int)startPosition.x, (int)startPosition.y] = true;
        }

        public void KingActions(bool[,] t)
        {
            /*
            //Back
            if (currentZ != 8)
            {
                t[currentX, currentZ + 1] = true;
            }
            */

            
            //Forward
            if (currentZ != 0)
            {
                t[currentX, currentZ - 1] = true;
            }

            
            //Left
            if (currentX != 0)
            {
                t[currentX + 1, currentZ] = true;
            }

            /*
            //Left
            if (currentX != 0)
            {
                t[currentX - 1, currentZ] = true;
            }
            
            //Diagonal left
            if (currentX != 0 && currentZ != 8)
            {
                t[currentX - 1, currentZ + 1] = true;
            }
            */

            /*
            //Diagonal right
            if (currentX != 8 && currentZ != 8)
            {
                t[currentX + 1, currentZ + 1] = true;
            }

            
            //Diagonal back left 
            if (currentX != 0 && currentZ != 0)
            {
                t[currentX - 1, currentZ - 1] = true;
            }
            */
            //Diagonal back right
            if (currentX != 8 && currentZ != 0)
            {
                t[currentX + 1, currentZ - 1] = true;
            }
        }

        private void BishopActions(bool[,] t)
        {
            //left up
            t[currentX + 1, currentZ - 1] = true;

            //right up
            t[currentX - 1, currentZ - 1] = true;
            t[currentX - 2, currentZ - 2] = true;
            t[currentX - 3, currentZ - 3] = true;
            t[currentX - 4, currentZ - 4] = true;

            //right down
            t[currentX - 1, currentZ + 1] = true;
            t[currentX - 2, currentZ + 2] = true;
            t[currentX - 3, currentZ + 3] = true;

            //left down
            t[currentX + 1, currentZ + 1] = true;
            t[currentX + 2, currentZ + 2] = true;

        }

        private void KnightActions(bool[,] t)
        {
            //left up
            t[currentX + 1, currentZ - 2] = true;
            //right up
            t[currentX - 1, currentZ - 2] = true;
            //right forward
            t[currentX - 2, currentZ - 1] = true;
            //left forward
            t[currentX + 2, currentZ - 1] = true;
            //right backward
            t[currentX - 2, currentZ + 1] = true;
            //left backward
            t[currentX + 2, currentZ + 1] = true;
            //right down
            t[currentX - 1, currentZ + 2] = true;
            //left down
            t[currentX + 1, currentZ + 2] = true;
        }

        private void PawnActions(bool[,] t)
        {
            //Forward
            if (currentZ != 1)
            {
                t[currentX, currentZ - 1] = true;
            }
        }
    }
}