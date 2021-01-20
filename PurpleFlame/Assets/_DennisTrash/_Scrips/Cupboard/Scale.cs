using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class Scale : MonoBehaviour
    {
        private Animator anim;
        private int weightLeft;
        private int weightRight;
        private int amountOfWeights = 0;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void AddWeightToScale(Weight weight, int side) //0=left 1=right
        {
            if(side == 0) { weightLeft = weight.heavyWeight; }
            if (side == 1) { weightRight = weight.heavyWeight; }
            amountOfWeights++;

            if (amountOfWeights == 2) { CheckNewWeight(); }
            else { CheckWeight(); }
        }

        private void CheckWeight()
        {
            if (weightLeft == 0 && weightRight > 0) { anim.SetBool("RightSolo", true); }
            else if (weightRight == 0 && weightLeft > 0) { anim.SetBool("LeftSolo", true); }
            else if (weightLeft > weightRight) { anim.SetBool("LeftHeavier", true); }
            else if (weightLeft < weightRight) { anim.SetBool("RightHeavier", true); }
        }

        private void CheckNewWeight()
        {
            if(weightLeft < weightRight)
            {
                anim.SetBool("LeftSolo", false);
                anim.SetBool("LeftHeavier", false);
            }
            else if(weightLeft > weightRight)
            {
                anim.SetBool("RightSolo", false);
                anim.SetBool("RightHeavier", false);
            }
            else if (weightLeft == weightRight)
            {
                anim.SetBool("LeftSolo", false);
                anim.SetBool("LeftHeavier", false);
                anim.SetBool("RightSolo", false);
                anim.SetBool("RightHeavier", false);
            }

            if (weightLeft < weightRight && weightLeft != 0) 
            { 
                anim.SetBool("RightHeavier", true);
                anim.SetBool("RightSolo", false);
            }
            else if(weightLeft < weightRight)
            {
                anim.SetBool("RightSolo", true);
                anim.SetBool("RightHeavier", false);
            }
            if(weightLeft > weightRight && weightRight != 0) 
            { 
                anim.SetBool("LeftHeavier", true);
                anim.SetBool("LeftSolo", false);
            }
            else if(weightLeft > weightRight)
            {
                anim.SetBool("LeftSolo", true);
                anim.SetBool("LeftHeavier", false);
            }
        }

        public void RemoveWeight(int side)
        {
            if (side == 0) { weightLeft = 0; }
            if (side == 1) { weightRight = 0; }
            amountOfWeights--;

            if(amountOfWeights == 1) { CheckNewWeight(); }
            else
            {
                anim.SetBool("LeftHeavier", false);
                anim.SetBool("RightHeavier", false);
                anim.SetBool("RightSolo", false);
                anim.SetBool("LeftSolo", false);

                CheckWeight();
            }
        }
    }
}