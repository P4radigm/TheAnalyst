using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public interface IPuzzle
    {
        void ActivatePressEvents();
        void ActivateReleaseEvents();
    }
}