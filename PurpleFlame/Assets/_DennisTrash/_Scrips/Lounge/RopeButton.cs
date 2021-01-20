using PurpleFlame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class RopeButton : MonoBehaviour
    {
        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void PressedButton()
        {
            anim.SetTrigger("ButtonPressed");
        }
    }
}