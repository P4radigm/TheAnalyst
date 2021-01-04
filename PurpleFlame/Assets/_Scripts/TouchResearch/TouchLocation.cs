using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class TouchLocation 
    {
        public int TouchId;
        public GameObject Circle;

        public TouchLocation(int touchId, GameObject circle)
        {
            TouchId = touchId;
            Circle = circle;
        }
    }
}
