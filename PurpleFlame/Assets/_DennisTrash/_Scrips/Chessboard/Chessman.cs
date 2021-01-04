using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public int currentX { set; get; }
    public int currentZ { set; get; }

    public void SetPosition(int x, int z)
    {
        currentX = x;
        currentZ = z;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[9, 9];
    }
}
