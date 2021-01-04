using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class MovableObject : MonoBehaviour, IMovable
    {
        public MoveAxis MoveAxis;

        public void Move()
        {

        }

        public MoveAxis GetDirectionAxis()
        {
            return MoveAxis;
        }
    }
}
