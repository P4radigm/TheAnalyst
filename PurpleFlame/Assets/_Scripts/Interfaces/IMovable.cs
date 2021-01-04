using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public enum MoveAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
    }
    public interface IMovable
    {
        void Move();
        MoveAxis GetDirectionAxis();
    }
}