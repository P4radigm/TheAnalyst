using PurpleFlame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : LeanDrag
{
    public Chessman[,] chessman { set; get; }

    [SerializeField] private float lengthBoard;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private List<ChessPiece> chessPieces = new List<ChessPiece>();

    [Header("End position chesspiece")]
    [SerializeField] private ChessPieceEnum endPiece;
    [SerializeField] private Vector2 endPosition;

    [Header("Puzzle Solved")]
    [SerializeField] private Animator mainDeskAnim;
    [SerializeField] private Animator typeAnim;
    [SerializeField] private SetCamColliderVisible sCCV;
    [SerializeField] private Transform chessBoardObj;
    [SerializeField] private GameObject chessPiecesObj;
    [SerializeField] private GameObject interactiveTrigger;

    private bool[,] allowedMoves { get; set; }
    private ChessPiece selectedPiece;
    private RaycastHit hit;
    private float tileSize;
    private float offset;
    private int piecesOnBoard = 0;
    private bool pieceSelected;
    private ChessPiece pieceMoved;
    private Coroutine typeRoutine;

    private void Start()
    {
        tileSize = lengthBoard / 8;
        offset = tileSize / 2;
    }

    private void Update()
    {
        if(pieceSelected && piecesOnBoard == 4)
        {
            PieceSelected();
        }
    }

    sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
    {
        base.OnFingerDown(finger);

        Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject.GetComponent<ChessPiece>())
            {
                selectedPiece = hit.collider.gameObject.GetComponent<ChessPiece>();
                if (!selectedPiece.onBoard) { SetOnBoard(selectedPiece); }

                selectedPiece.SetPosition((int)selectedPiece.startPosition.x, (int)selectedPiece.startPosition.y);

                if(piecesOnBoard != 4) { return; }
                if (pieceMoved == selectedPiece || pieceMoved == null)
                {
                    allowedMoves = selectedPiece.PossibleMove();
                    HighLightMoves.Instance.HighlightAllowedMoves(allowedMoves);
                }
            }
        }
    }

    private void SetOnBoard(ChessPiece selectedPiece)
    {
        piecesOnBoard++;
        selectedPiece.onBoard = true;
        Vector3 startPosition = GetCurrentTilePosition(selectedPiece.startPosition.x, selectedPiece.startPosition.y);
        selectedPiece.transform.position = startPosition;
    }

    sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
    {
        base.OnFingerUp(finger);
        //if (pieceMoved != null && selectedPiece.chessPiece != pieceMoved.chessPiece) { selectedPiece = null; }

        if (selectedPiece != null)
        {
            pieceSelected = true;
        }
        else
        {
            pieceSelected = false;
        }
    }

    private void PieceSelected()
    {
        if (touchingFingers.Count > 0)
        {
            if(selectedPiece != null) { allowedMoves = selectedPiece.PossibleMove(); }
            HighLightMoves.Instance.HideHighLights();
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if(pieceMoved == selectedPiece || pieceMoved == null) 
                {
                    Vector2 moveToTile = ClickedTile(hit.point.x, hit.point.z);

                    if (allowedMoves[(int)moveToTile.x, (int)moveToTile.y] == true)
                    {
                        selectedPiece.transform.position = GetCurrentTilePosition(moveToTile.x, moveToTile.y);

                        if (moveToTile == endPosition && selectedPiece.chessPiece == endPiece)
                        {
                            Debug.Log("SCHAAK MAT");
                            ChessSolved();
                        }

                        CheckIfPieceMoved();
                    }
                    else
                    {
                        CheckIfPieceMoved();
                        selectedPiece = null;
                    }
                }
            }
        }
    }

    private void CheckIfPieceMoved()
    {
        if (selectedPiece == null) { return; }

        if (!selectedPiece.unMoved)
        {
            pieceMoved = selectedPiece;
        }
        else
        {
            pieceMoved = null;
        }
    }

    private void ChessSolved()
    {
        chessPiecesObj.transform.parent = chessBoardObj;
        mainDeskAnim.SetTrigger("ChessSolved");
        //Destroy(interactiveTrigger);
        if(typeRoutine != null) { return; }
        typeRoutine = StartCoroutine(StartTypeAnim());
    }

    public Vector3 GetCurrentTilePosition(float x, float z)
    {
        Vector3 currentTilePosition = new Vector3(startPos.x + ((int)x * tileSize - offset), startPos.y, startPos.z + ((int)z * tileSize - offset));
        return currentTilePosition;
    }

    public Vector2 ClickedTile(float xPos, float zPos)
    {
        Vector2 clickedTile = new Vector2(0,0);
        for (int x = 0; x <= 8; x++)
        {
            for (int z = 0; z <= 8; z++)
            {
                if(startPos.x + (x * tileSize) >= xPos && startPos.x + (x * tileSize - tileSize) <= xPos &&
                   startPos.z + (z * tileSize) >= zPos && startPos.z + (z * tileSize - tileSize) <= zPos)
                {
                    clickedTile = new Vector2(x, z);
                }
            }
        }
        return clickedTile;
    }

    private IEnumerator StartTypeAnim()
    {
        yield return new WaitForSeconds(7.7f);

        typeAnim.SetTrigger("WeightPuzzleSolved");

        yield return new WaitForSeconds(2f);

        sCCV.SetCamColVisible();
    }

    #region Singleton
    private static Chessboard instance;

    private void Awake()
    {
        instance = this;
    }

    public static Chessboard Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Chessboard();
            }

            return instance;
        }
    }
    #endregion
}