using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPages : MonoBehaviour
{
    [SerializeField] private GameObject longPaper;
    [SerializeField] private GameObject shortPaper;

    private void Awake()
    {
        shortPaper.SetActive(true);
        longPaper.SetActive(false);
    }
    
    public void SwitchPaper(bool focusedTypeWriter)
    {
        if (focusedTypeWriter)
        {
            shortPaper.SetActive(false);
            longPaper.SetActive(true);
        }
        else
        {
            shortPaper.SetActive(true);
            longPaper.SetActive(false);
        }
    }
}
