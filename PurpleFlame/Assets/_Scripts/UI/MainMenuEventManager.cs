using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEventManager : MonoBehaviour
{
    public static MainMenuEventManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public event Action OnPinchedInwards;
    public void PinchedInwards()
    {
        if(OnPinchedInwards != null)
        {
            OnPinchedInwards();
        }
    }
}
