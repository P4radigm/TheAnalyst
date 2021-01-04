using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutMenu : MonoBehaviour
{
    [SerializeField] private GameObject outroPanel;
    public void ShowEndText()
    {
        outroPanel.SetActive(true);
    }
}
