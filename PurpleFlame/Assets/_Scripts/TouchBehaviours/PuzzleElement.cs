using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TouchBehaviours
{
    public class PuzzleElement : MonoBehaviour, IPuzzle
    {
        [SerializeField] private UnityEvent puzzleElementPressedEvents;
        [SerializeField] private UnityEvent puzzleElementReleasedEvents;

        public void ActivatePressEvents()
        {
            puzzleElementPressedEvents.Invoke();
        }

        public void ActivateReleaseEvents()
        {
            puzzleElementReleasedEvents.Invoke();
        }
    }
}
