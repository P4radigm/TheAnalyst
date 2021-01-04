using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class Act1_AnimationManager : MonoBehaviour
    {
        public Animator StairsAnimator;
        public GameObject OldStairs;

        private void OnEnable()
        {
            EventManager<bool>.AddHandler(EVENT.Act1_Brievenbus_DONE, ActivateAnimation);
        }

        public void ActivateAnimation(bool value = true)
        {
            StairsAnimator.enabled = value;
            StairsAnimator.GetComponent<Collider>().enabled = false;
            //OldStairs.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager<bool>.RemoveHandler(EVENT.Act1_Brievenbus_DONE, ActivateAnimation);
        }
    }
}
