using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObject : MonoBehaviour
{
    [SerializeField] private GameObject[] objectToActivate;

    void ActivateObjectTrue()
    {
        for (int i = 0; i < objectToActivate.Length; i++)
        {
            objectToActivate[i].SetActive(true);
        }
        
    }
}
