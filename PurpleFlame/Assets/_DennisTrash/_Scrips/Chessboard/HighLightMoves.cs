using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightMoves : MonoBehaviour
{
    [SerializeField] private GameObject highlightPrefab;
    private List<GameObject> highlights = new List<GameObject>();

    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if(go == null)
        {
            go = Instantiate(highlightPrefab, this.transform);
            highlights.Add(go);
        }
        return go;
    }

    public void HighlightAllowedMoves(bool[,] moves)
    {
        for (int x = 0; x < 9; x++)
        {
            for (int z = 0; z < 9; z++)
            {
                if(moves[x, z])
                {
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = Chessboard.Instance.GetCurrentTilePosition(x, z);
                    //go.transform.position = new Vector3(go.transform.position.x + 0.01f, go.transform.position.y + 0.08f, go.transform.position.z + 0.015f);
                    go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
                }
            }
        }
    }

    public void HideHighLights()
    {
        foreach (GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }

    #region Singleton
    public static HighLightMoves Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }
    #endregion
}
